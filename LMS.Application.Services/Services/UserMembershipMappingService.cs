using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Transection;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;
using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Common.Security;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class UserMembershipMappingService : IUserMembershipMappingService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly HttpContext? _httpContext;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UserMembershipMapping> _logger;

    public UserMembershipMappingService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService, INotificationService notificationService, ILogger<UserMembershipMapping> logger)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _validationService = validationService;
        _httpContext = httpContextAccessor.HttpContext;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<PaginatedResponseDto<T>> GetUserMembershipAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        isActive = authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true;

        var userMembershipQuery = _repositoryManager.UserMembershipMappingRepository
            .GetUserMembershipOrderByActiveStatus(isActive: isActive)
            .Where(x => (!userId.HasValue || x.UserId == (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) ? authUserId : userId))
                && (isActive != true || x.ExpirationDate.Date >= DateTimeOffset.UtcNow.Date));

        var totalCount = await userMembershipQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var userMemberships = await userMembershipQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = userMemberships
        };
    }

    public async Task<List<GetUserMembersipListDto>> GetLoginUserMembershipAsync()
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var userMembershipQuery = _repositoryManager.UserMembershipMappingRepository.GetAllAsync().Where(x => x.UserId == authUserId);

        var activeMemberships = await userMembershipQuery
            .Where(x => x.IsActive && x.ExpirationDate >= DateTimeOffset.UtcNow)
            .ProjectTo<GetUserMembersipListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var lastExpirationMembership = await userMembershipQuery
            .Where(m => m.ExpirationDate < DateTimeOffset.UtcNow)
            .OrderByDescending(m => m.ExpirationDate)
            .ProjectTo<GetUserMembersipListDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (lastExpirationMembership is not null)
            activeMemberships.Add(lastExpirationMembership);

        return activeMemberships.OrderBy(x => x.ExpirationDate).ToList();
    }

    public async Task<T> GetUserMembershipByIdAsync<T>(long id) where T : class
    {
        var userMembership = await _repositoryManager.UserMembershipMappingRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return userMembership ?? Activator.CreateInstance<T>();
    }

    public async Task<Dictionary<string, bool>> GetUserEligibilityToAdd()
    {
        var users = await _repositoryManager.UserRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.RoleId == (long)RoleListEnum.User)
            .Select(x => x.Id)
            .ToListAsync();

        var userMembership = await _repositoryManager.UserMembershipMappingRepository
            .GetAllAsync(isActive: true)
            .Select(x => new
            {
                UserId = x.UserId,
                ExpirationDate = x.ExpirationDate
            })
            .ToListAsync();

        var maxActiveMembership = await GetMaxActiveMembership();

        Dictionary<string, bool> userEligibility = users.ToDictionary(
            id => id.ToString(),
            id => userMembership.Count(m => m.UserId == id && m.ExpirationDate > DateTime.UtcNow) < maxActiveMembership
        );

        return userEligibility;
    }

    public async Task<byte[]> ExportUserMembershipData()
    {
        var userMemberships = await GetUserMembershipAsync<ExportUserMembershipDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "User-membership", userMemberships.Data } });
    }

    public async Task AddOrUpgradeUserMembershipAsync(long membershipId, bool IsUpgradePlan = false, long? userId = null)
    {
        bool isLogin = long.TryParse(_httpContext?.GetUserId(), out long authUserId);
        if (_httpContext!.GetUserRole().Equals(RoleListEnum.User.ToString(), comparisonType: StringComparison.CurrentCultureIgnoreCase))
            userId = authUserId;

        if (!isLogin || userId == null)
            throw new BadRequestException("User need to login for this");

        if (!(await _repositoryManager.UserRepository.AnyAsync(x => x.IsActive && x.Id == userId)))
            throw new BadRequestException("This user is not available");

        var membership = await _repositoryManager.MembershipRepository
            .GetByIdAsync(membershipId).FirstOrDefaultAsync();

        if (membership == null)
            throw new BadRequestException("Selected Membership not found");

        var existUserMemberships = await _repositoryManager.UserMembershipMappingRepository.FindByCondition(x => x.UserId == userId && x.ExpirationDate.Date >= DateTimeOffset.UtcNow.Date && x.IsActive).ToListAsync();

        long maxActiveMembership = await GetMaxActiveMembership();

        if (existUserMemberships.Count() >= maxActiveMembership && !IsUpgradePlan)
            throw new BadRequestException("Already have max number of plan, can't add new one");

        if (!existUserMemberships.Any() && IsUpgradePlan)
            throw new BadRequestException("You haven't any active plan, Upgrade Plan doesn't work");

        var latestUserPlan = existUserMemberships
            .OrderBy(x => x.EffectiveStartDate)
            .LastOrDefault();

        long nextPlanActivationTimeInMinutes = await GetNextPlanActivationTimeInMinutes();

        var userMembership = new UserMembershipMapping();
        userMembership.Id = 0;
        userMembership.UserId = (long)userId;
        userMembership.MembershipId = membershipId;
        userMembership.EffectiveStartDate = IsUpgradePlan
            ? latestUserPlan!.EffectiveStartDate < DateTimeOffset.UtcNow ? DateTimeOffset.UtcNow : latestUserPlan!.EffectiveStartDate
            : latestUserPlan?.ExpirationDate.AddMinutes(nextPlanActivationTimeInMinutes) ?? DateTimeOffset.UtcNow;
        userMembership.ExpirationDate = userMembership.EffectiveStartDate.AddDays(membership.Duration);
        userMembership.MembershipCost = membership.Cost;
        userMembership.Discount = membership.Discount;
        userMembership.ReservationLimit = membership.ReservationLimit;
        userMembership.BorrowLimit = membership.BorrowLimit;
        userMembership.IsActive = true;

        if (IsUpgradePlan)
        {
            latestUserPlan!.IsActive = false;
            _repositoryManager.UserMembershipMappingRepository.Update(latestUserPlan!);
        }
        await _repositoryManager.UserMembershipMappingRepository.AddAsync(userMembership);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();

        var newUserMembership = await _repositoryManager.UserMembershipMappingRepository
            .GetByIdAsync(userMembership.Id)
            .Include(x => x.User)
            .FirstOrDefaultAsync();

        if (newUserMembership is not null)
        {
            if (!IsUpgradePlan)
                await _notificationService.DispatchEmail(NotificationTypeEnum.NewMembership, newUserMembership);
            else
            {
                var oldUserMembershipType = await _repositoryManager.UserMembershipMappingRepository
                    .GetByIdAsync(latestUserPlan!.Id)
                    .Include(x => x.Membership)
                    .Select(x => x.Membership.Type)
                    .FirstOrDefaultAsync();

                await _notificationService.DispatchEmail(NotificationTypeEnum.UpgradeMembership, new { User = newUserMembership.User, NewMembershipType = newUserMembership.Membership.Type, OldMembershipType = oldUserMembershipType, EffectiveStartDate = newUserMembership.EffectiveStartDate, ExpirationDate = newUserMembership.ExpirationDate });
            }
        }
        else
            _logger.LogWarning($"Just added user membership not found in table while tring to send e-mail notification : {userMembership.Id}");
    }

    public async Task UpdateUserMembershipAsync(UpdateUserMembershipDto userMembership)
    {
        _validationService.Validate<UpdateUserMembershipDto>(userMembership);

        var existUserMembership = await _repositoryManager.UserMembershipMappingRepository.GetByIdAsync(id: userMembership.Id).FirstOrDefaultAsync();

        if (existUserMembership == null)
            throw new BadRequestException("This User Membership is not available");

        var userPassword = await _repositoryManager.UserRepository.GetByIdAsync(id: existUserMembership.UserId)
            .Select(x => new
            {
                PasswordHash = x.PasswordHash,
                PasswordSolt = x.PasswordSolt
            })
            .FirstOrDefaultAsync();

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (isLogin && authUserRole.Equals(nameof(RoleListEnum.Librarian), StringComparison.InvariantCultureIgnoreCase))
        {
            if (userPassword != null
                && userMembership.UserPassword is not null
                && PasswordHasher.VerifyPassword(password: userMembership.UserPassword, passwordHash: userPassword.PasswordHash, passwordSolt: userPassword.PasswordSolt))
            {
                existUserMembership.BorrowLimit = userMembership.BorrowLimit;
                existUserMembership.ReservationLimit = userMembership.ReservationLimit;
            }

            existUserMembership.MembershipCost = userMembership.MembershipCost;
            existUserMembership.Discount = userMembership.Discount;
            existUserMembership.PaidAmount = userMembership.PaidAmount;
        }
        else if (isLogin && authUserRole.Equals(nameof(RoleListEnum.Admin), StringComparison.InvariantCultureIgnoreCase))
        {
            var existActiveMembershipInSelectedTime = await _repositoryManager.UserMembershipMappingRepository
                .GetAllAsync(isActive: true)
                .AnyAsync(x => x.Id != existUserMembership.Id && x.UserId == existUserMembership.UserId && x.EffectiveStartDate <= userMembership.ExpirationDate && x.ExpirationDate >= userMembership.EffectiveStartDate);

            if (existActiveMembershipInSelectedTime)
                throw new BadRequestException("In this interval already one membership is Active");

            if (existUserMembership.EffectiveStartDate <= DateTimeOffset.UtcNow && existUserMembership.ExpirationDate >= DateTimeOffset.UtcNow)
            {
                if (userPassword != null
                && userMembership.UserPassword is not null
                && PasswordHasher.VerifyPassword(password: userMembership.UserPassword, passwordHash: userPassword.PasswordHash, passwordSolt: userPassword.PasswordSolt))
                {
                    existUserMembership.BorrowLimit = userMembership.BorrowLimit;
                    existUserMembership.ReservationLimit = userMembership.ReservationLimit;
                    existUserMembership.EffectiveStartDate = userMembership.EffectiveStartDate;
                    existUserMembership.ExpirationDate = userMembership.ExpirationDate;
                }
                else
                    throw new BadRequestException("Given User password is not valid");
            }
            else
            {
                existUserMembership.BorrowLimit = userMembership.BorrowLimit;
                existUserMembership.ReservationLimit = userMembership.ReservationLimit;
                existUserMembership.EffectiveStartDate = userMembership.EffectiveStartDate;
                existUserMembership.ExpirationDate = userMembership.ExpirationDate;
            }

            existUserMembership.MembershipCost = userMembership.MembershipCost;
            existUserMembership.Discount = userMembership.Discount;
            existUserMembership.PaidAmount = userMembership.PaidAmount;
        }

        if (isLogin && (authUserRole.Equals(nameof(RoleListEnum.Admin), StringComparison.InvariantCultureIgnoreCase) || authUserRole.Equals(nameof(RoleListEnum.Librarian), StringComparison.InvariantCultureIgnoreCase)))
        {
            _repositoryManager.UserMembershipMappingRepository.Update(entity: existUserMembership);
            _repositoryManager.UnitOfWork.SaveChanges();
        }
    }

    public async Task DeleteUserMembershipAsync(long id)
    {
        var existUserMembership = await _repositoryManager.UserMembershipMappingRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existUserMembership == null)
            throw new BadRequestException("This UserMembership is already not exist");

        existUserMembership.IsActive = false;

        _repositoryManager.UserMembershipMappingRepository.Update(entity: existUserMembership);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task DeleteAllUserMembershipAsync(long userId)
    {
        var existUserMemberships = await _repositoryManager.UserMembershipMappingRepository.FindByCondition(x => x.UserId == userId && x.IsActive).ToListAsync();

        if (existUserMemberships.Any())
        {
            existUserMemberships.ForEach(x =>
            {
                x.IsActive = false;
            });

            _repositoryManager.UserMembershipMappingRepository.UpdateRange(entities: existUserMemberships);
            _repositoryManager.UnitOfWork.SaveChanges();
        }
    }

    public async Task PermanentDeleteUserMembershipAsync(long id)
    {
        var existUserMembership = await _repositoryManager.UserMembershipMappingRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existUserMembership == null)
            throw new BadRequestException("This UserMembership is already not exist");

        _repositoryManager.UserMembershipMappingRepository.Remove(entity: existUserMembership);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task RemoveExpiredUserMembershipAsync(bool forLogin = false)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var expiredUserMemberships = await _repositoryManager.UserMembershipMappingRepository
            .FindByCondition(x => x.IsActive && (forLogin == false || forLogin == true && x.UserId == authUserId) && x.ExpirationDate <= DateTimeOffset.UtcNow)
            .ToListAsync();

        if (expiredUserMemberships.Any())
        {
            expiredUserMemberships.ForEach(x => x.IsActive = false);

            _repositoryManager.UserMembershipMappingRepository.UpdateRange(entities: expiredUserMemberships);
            _repositoryManager.UnitOfWork.SaveChanges();
        }
    }

    public async Task DispatchEmailForMembershipDueRemainder()
    {
        // Get all active memberships that haven't expired yet
        var userMembership = await _repositoryManager.UserMembershipMappingRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.ExpirationDate.Date >= DateTimeOffset.UtcNow.Date)
            .ToListAsync();

        // Get users who already have a future plan starting today or later
        var usersWithFuturePlans = userMembership
            .Where(x => x.EffectiveStartDate.Date >= DateTimeOffset.UtcNow.Date)
            .Select(x => x.UserId)
            .Distinct()
            .ToList();

        // Filter memberships that expire today or within 3 days, and exclude users with future plans
        var filteredUserMembership = userMembership
            .Where(x => !usersWithFuturePlans.Contains(x.UserId) 
                && (x.ExpirationDate.Date == DateTimeOffset.UtcNow.Date || x.ExpirationDate.Date == DateTimeOffset.UtcNow.AddDays(3).Date))
            .Cast<object>()
            .ToList();

        await _notificationService.DispatchMultipleEmail(NotificationTypeEnum.MembershipDue, filteredUserMembership);
    }

    #region Private section
    private async Task<long> GetMaxActiveMembership()
    {
        long.TryParse((await _repositoryManager.ConfigRepository
                .GetByKeyNameAsync("MaxActiveMembership")
                .FirstOrDefaultAsync())?.KeyValue ?? "0", out long maxActiveMembership);
        return maxActiveMembership;
    }

    private async Task<long> GetNextPlanActivationTimeInMinutes()
    {
        long.TryParse((await _repositoryManager.ConfigRepository
                .GetByKeyNameAsync("NextPlanActivationTimeInMinutes")
                .FirstOrDefaultAsync())?.KeyValue ?? "0", out long nextPlanActivationTimeInMinutes);
        return nextPlanActivationTimeInMinutes;
    }
    #endregion
}
