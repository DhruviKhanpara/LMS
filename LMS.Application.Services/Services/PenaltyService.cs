using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class PenaltyService : IPenaltyService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly HttpContext? _httpContext;
    private readonly ILogger<PenaltyService> _logger;

    public PenaltyService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService, ILogger<PenaltyService> logger)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _validationService = validationService;
        _httpContext = httpContextAccessor.HttpContext;
        _logger = logger;
    }

    public async Task<PaginatedResponseDto<T>> GetAllPenaltyAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null, long? penaltyTypeId = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var penaltyQuery = _repositoryManager.PenaltyRepository
            .GetPenaltyOrderByDefault(isActive: _httpContext!.GetUserRole().Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true)
        .Where(x => (!userId.HasValue || x.UserId == (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) ? authUserId : userId)) && (!penaltyTypeId.HasValue || x.PenaltyTypeId == penaltyTypeId))
        .Where(x => authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) || x.User.RoleId == (long)RoleListEnum.User);

        var totalCount = await penaltyQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var penaltys = await penaltyQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = penaltys
        };
    }

    public async Task<PaginatedResponseDto<GetPenaltyDto>> GetUserPenaltyAsync(int? pageSize = null, int? pageNumber = null)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var penaltyQuery = _repositoryManager.PenaltyRepository
            .GetPenaltyOrderByDefault(isActive: true)
            .Where(x => x.UserId == authUserId);

        var unpaidPenalty = await penaltyQuery
            .Where(x => x.StatusId == (long)FineStatusEnum.UnPaid)
            .ProjectTo<GetPenaltyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        penaltyQuery = penaltyQuery.Where(x => x.StatusId != (long)FineStatusEnum.UnPaid);

        var totalCount = await penaltyQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var penaltys = await penaltyQuery
            .ProjectTo<GetPenaltyDto>(_mapper.ConfigurationProvider)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<GetPenaltyDto>()
        {
            Pagination = paginationModel,
            Data = unpaidPenalty,
            Data1 = penaltys
        };
    }

    public async Task<T> GetPenaltyByIdAsync<T>(long id) where T : class
    {
        var penalty = await _repositoryManager.PenaltyRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return penalty ?? Activator.CreateInstance<T>();
    }

    public async Task<bool> HavePendingPenalty()
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        if (!isLogin)
            throw new BadRequestException("User needs to be login");

        return await _repositoryManager.PenaltyRepository.AnyAsync(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid && x.UserId == authUserId);
    }

    public async Task<byte[]> ExportPenaltyData()
    {
        var penalty = await GetAllPenaltyAsync<ExportPenaltyDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Penalty", penalty.Data } });
    }

    public async Task AddPenaltyAsync(AddPenaltyDto penalty)
    {
        _validationService.Validate<AddPenaltyDto>(penalty);

        var mappedPenalty = _mapper.Map<Penalty>(penalty);
        await _repositoryManager.PenaltyRepository.AddAsync(mappedPenalty);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdatePenaltyAsync(UpdatePenaltyDto penalty)
    {
        _validationService.Validate<UpdatePenaltyDto>(penalty);

        var existPenalty = await _repositoryManager.PenaltyRepository.GetByIdAsync(id: penalty.Id).FirstOrDefaultAsync();

        if (existPenalty == null)
            throw new BadRequestException("This Penalty is not available");

        if (penalty.TransectionId != null && existPenalty.StatusId == (long)FineStatusEnum.UnPaid && penalty.StatusId != (long)FineStatusEnum.UnPaid)
        {
            if (!new[] { (long)TransectionStatusEnum.Renewed, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(penalty.TransectionStatusId ?? 0))
            {
                _logger.LogWarning($"Invalid transaction status: Received unexpected TransectionStatusId '{penalty.TransectionStatusId}'. Only statuses from [Renew, Return, ClaimedLost] are supported for linked transactions.");
                throw new BadRequestException("Undefined Transection status");
            }

            var existTransection = await _repositoryManager.TransectionRepository.GetByIdAsync(id: (long)penalty.TransectionId).FirstOrDefaultAsync();

            if (existTransection == null)
                throw new BadRequestException("This Transection is not available");

            existTransection.StatusId = penalty.TransectionStatusId ?? 0;

            if (penalty.TransectionStatusId == (long)TransectionStatusEnum.Returned)
                existTransection.ReturnDate = DateTimeOffset.UtcNow;

            if (penalty.TransectionStatusId == (long)TransectionStatusEnum.Renewed)
            {
                existTransection.RenewDate = DateTimeOffset.UtcNow;
                existTransection.DueDate = DateTimeOffset.UtcNow.AddDays(await GetBorrowDueDays());
            }
            _repositoryManager.TransectionRepository.Update(entity: existTransection);
        }

        var mappedPenalty = _mapper.Map(penalty, existPenalty);

        _repositoryManager.PenaltyRepository.Update(entity: mappedPenalty);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task RemovePenaltyAsync(long id)
    {
        var authUserRole = _httpContext!.GetUserRole();
        if (authUserRole.Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("You can't remove the penalty");

        var existPenalty = await _repositoryManager.PenaltyRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existPenalty == null)
            throw new BadRequestException("This Penalty is already not exist");

        if (!authUserRole.Equals(nameof(RoleListEnum.Admin), StringComparison.InvariantCultureIgnoreCase) && existPenalty.StatusId == (long)FineStatusEnum.UnPaid)
            throw new BadRequestException("you haven't permission to remove this penalty, this is un-paid");

        existPenalty.IsActive = false;

        _repositoryManager.PenaltyRepository.Update(entity: existPenalty);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task PermanentDeletePenaltyAsync(long id)
    {
        var existPenalty = await _repositoryManager.PenaltyRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existPenalty == null)
            throw new BadRequestException("This Penalty is already not exist");

        _repositoryManager.PenaltyRepository.Remove(entity: existPenalty);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task CalculatePenaltyForHoldingBooks(bool forLogin = false)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        (int previousLimitCarryoverDays, int membershipExpiryBufferDays) = await GetBufferTimeFromConfig();

        var usersWithOverHolding = _repositoryManager.UserRepository
            .FindByCondition(x => x.IsActive && (forLogin == false || forLogin == true && x.Id == authUserId))
            .Select(x => new
            {
                UserId = x.Id,
                ActiveTransectionCount = x.Transections.Count(t =>
                    t.IsActive &&
                    !new[] {
                        (long)TransectionStatusEnum.Returned,
                        (long)TransectionStatusEnum.Cancelled,
                        (long)TransectionStatusEnum.ClaimedLost
                    }.Contains(t.StatusId)
                ),
                ActiveMembership = x.UserMemberships
                    .Where(m =>
                        m.IsActive &&
                        m.EffectiveStartDate < DateTimeOffset.UtcNow &&
                        m.ExpirationDate > DateTimeOffset.UtcNow
                    )
                    .FirstOrDefault(),
                Penalties = x.Penalties.Where(p =>
                    p.IsActive &&
                    p.StatusId == (long)FineStatusEnum.UnPaid &&
                    new[] { (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership, (long)PenaltyTypeEnum.ExtraHoldings }.Contains(p.PenaltyTypeId)
                ),
                LastExpireMembershipDate = x.UserMemberships
                    .Where(m => m.ExpirationDate <= DateTimeOffset.UtcNow)
                    .OrderByDescending(m => m.ExpirationDate)
                    .Select(m => m.ExpirationDate)
                    .FirstOrDefault(),
            })
            .AsEnumerable()
            .Where(x => x.ActiveTransectionCount > (x.ActiveMembership?.BorrowLimit ?? 0))
            .ToList();

        var overHoldingWithActiveMembership = usersWithOverHolding
            .Where(x => x.ActiveMembership != null
                && (DateTimeOffset.UtcNow.Date - x.ActiveMembership.EffectiveStartDate.Date).Days > previousLimitCarryoverDays
                && (x.Penalties == null
                    || !x.Penalties.Any()
                    || x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.ActiveMembership.EffectiveStartDate.Date).Days - previousLimitCarryoverDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.ExtraHoldings)))
            .ToList();

        var overHoldingWithExpiredMembership = usersWithOverHolding
            .Where(x => x.ActiveMembership == null
                && x.LastExpireMembershipDate != default
                && (DateTimeOffset.UtcNow.Date - x.LastExpireMembershipDate.Date).Days > membershipExpiryBufferDays
                && (x.Penalties == null ||
                    !x.Penalties.Any() ||
                    x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.LastExpireMembershipDate.Date).Days - membershipExpiryBufferDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership)))
            .ToList();

        var usersWithNoMembershipEver = usersWithOverHolding
            .Where(x => x.ActiveMembership == null && x.LastExpireMembershipDate == default)
            .Select(x => x.UserId)
            .ToList();

        _logger.LogWarning($"{String.Join(",", usersWithNoMembershipEver)} users have book holding but haven't any purchase membership data found for them");

        var usersWithPenaltyInfo = overHoldingWithActiveMembership
            .Select(x => new
            {
                x.UserId,
                Penalties = x.Penalties,
                OverHoldCount = x.ActiveTransectionCount - (int)(x.ActiveMembership?.BorrowLimit ?? 0),
                OverdueDays = (DateTimeOffset.UtcNow.Date - x.ActiveMembership!.EffectiveStartDate.Date).Days - previousLimitCarryoverDays,
                PenaltyType = (long)PenaltyTypeEnum.ExtraHoldings,
                Description = $"Hold {x.ActiveTransectionCount - (int)(x.ActiveMembership?.BorrowLimit ?? 0)} more books than current plan limit"
            })
            .Concat(overHoldingWithExpiredMembership.Select(x => new
            {
                x.UserId,
                Penalties = x.Penalties,
                OverHoldCount = x.ActiveTransectionCount,
                OverdueDays = (DateTimeOffset.UtcNow.Date - x.LastExpireMembershipDate.Date).Days - membershipExpiryBufferDays,
                PenaltyType = (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership,
                Description = $"Hold {x.ActiveTransectionCount} books but user hasn't active membership"
            }))
            .ToList();

        var expiredTransection = _repositoryManager.TransectionRepository
            .FindByCondition(x => x.IsActive
                && (forLogin == false || forLogin == true && x.Id == authUserId)
                && !new[] { (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.ClaimedLost }.Contains(x.StatusId)
                && x.DueDate.Date < DateTimeOffset.UtcNow.Date)
            .Include(x => x.Penalties.Where(x => x.IsActive && x.PenaltyTypeId == (long)PenaltyTypeEnum.LateReturnRenew && x.StatusId == (long)FineStatusEnum.UnPaid))
            .AsEnumerable()
            .Where(x => x.Penalties == null
                || !x.Penalties.Any()
                || x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.DueDate.Date).Days
                    && p.StatusId == (long)FineStatusEnum.UnPaid
                    && p.PenaltyTypeId == (long)PenaltyTypeEnum.LateReturnRenew))
            .ToList();

        var penadingLostClaimPenalty = _repositoryManager.TransectionRepository
            .FindByCondition(x => x.IsActive
                && (forLogin == false || forLogin == true && x.Id == authUserId)
                && x.StatusId == (long)TransectionStatusEnum.ClaimedLost
                && x.LostClaimDate.HasValue)
            .Include(x => x.Penalties.Where(x => x.IsActive && x.PenaltyTypeId == (long)PenaltyTypeEnum.LostBook && x.StatusId == (long)FineStatusEnum.UnPaid))
            .AsEnumerable()
            .Where(x => x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.LostClaimDate!.Value.Date).Days
                    && p.StatusId == (long)FineStatusEnum.UnPaid
                    && p.PenaltyTypeId == (long)PenaltyTypeEnum.LostBook))
            .ToList();


        List<Penalty> penaltyToUpdate = new List<Penalty>();
        List<Transection> transectionToUpdate = new List<Transection>();

        (int basePenaltyPerDay, int increaseValue, int intervalDays, string increaseType) = await GetPenaltyInfoFromConfig();

        if (!usersWithPenaltyInfo.IsNullOrEmpty() && usersWithPenaltyInfo.Any())
        {
            usersWithPenaltyInfo.ForEach(x =>
            {
                var penaltyOfUser = x.Penalties?.Where(p =>
                        p.StatusId == (long)FineStatusEnum.UnPaid
                        && p.OverDueDays != x.OverdueDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.ExtraHoldings
                    ).FirstOrDefault();

                var amount = CalculateDynamicPenalty(overdueDays: x.OverdueDays, lastCalculatedDay: penaltyOfUser?.OverDueDays ?? 0, basePenalty: basePenaltyPerDay, increaseType: increaseType, increaseValue: increaseValue, intervalDays: intervalDays, penalizedItemCount: x.OverHoldCount);

                if (penaltyOfUser is null)
                {
                    penaltyOfUser = new Penalty()
                    {
                        UserId = x.UserId,
                        TransectionId = null,
                        StatusId = (long)FineStatusEnum.UnPaid,
                        PenaltyTypeId = x.PenaltyType,
                        Description = x.Description,
                        Amount = amount,
                        OverDueDays = x.OverdueDays,
                    };
                }
                else
                {
                    penaltyOfUser.Amount += amount;
                    penaltyOfUser.OverDueDays = x.OverdueDays;
                }

                penaltyToUpdate.Add(penaltyOfUser);
            });
        }

        if (!expiredTransection.IsNullOrEmpty() && expiredTransection.Any())
        {
            expiredTransection.ForEach(x =>
            {
                var overdueDays = (DateTimeOffset.UtcNow.Date - x.DueDate.Date).Days;

                var penaltyOfTransection = x.Penalties?.Where(p =>
                        p.StatusId == (long)FineStatusEnum.UnPaid
                        && p.OverDueDays != overdueDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.LateReturnRenew
                    ).FirstOrDefault();

                var amount = CalculateDynamicPenalty(overdueDays: overdueDays, lastCalculatedDay: penaltyOfTransection?.OverDueDays ?? 0, basePenalty: basePenaltyPerDay, increaseType: increaseType, increaseValue: increaseValue, intervalDays: intervalDays);

                if (penaltyOfTransection is null)
                {
                    penaltyOfTransection = new Penalty()
                    {
                        UserId = x.UserId,
                        TransectionId = x.Id,
                        StatusId = (long)FineStatusEnum.UnPaid,
                        PenaltyTypeId = (long)PenaltyTypeEnum.LateReturnRenew,
                        Description = "Due date is over and still not return or renew the book",
                        Amount = amount,
                        OverDueDays = overdueDays,
                    };
                }
                else
                {
                    penaltyOfTransection.Amount += amount;
                    penaltyOfTransection.OverDueDays = overdueDays;
                }

                penaltyToUpdate.Add(penaltyOfTransection);

                if (x.StatusId != (long)TransectionStatusEnum.Overdue)
                {
                    x.Penalties = null;
                    x.StatusId = (long)TransectionStatusEnum.Overdue;
                    transectionToUpdate.Add(x);
                }
            });
        }

        if (!penadingLostClaimPenalty.IsNullOrEmpty() && penadingLostClaimPenalty.Any())
        {
            penadingLostClaimPenalty.ForEach(x =>
            {
                var overdueDays = (DateTimeOffset.UtcNow.Date - x.LostClaimDate!.Value.Date).Days;

                var penaltyOfTransection = x.Penalties?.Where(p =>
                        p.StatusId == (long)FineStatusEnum.UnPaid
                        && p.OverDueDays != overdueDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.LostBook
                    ).FirstOrDefault();

                var amount = CalculateDynamicPenalty(overdueDays: overdueDays, lastCalculatedDay: penaltyOfTransection?.OverDueDays ?? 0, basePenalty: basePenaltyPerDay, increaseType: increaseType, increaseValue: increaseValue, intervalDays: intervalDays);

                if (penaltyOfTransection is null)
                {
                    _logger.LogWarning($"Transection with ClaimLost status and without penalty found: TransectionId = {x.Id}");
                    penaltyOfTransection = new Penalty()
                    {
                        UserId = x.UserId,
                        TransectionId = x.Id,
                        StatusId = (long)FineStatusEnum.UnPaid,
                        PenaltyTypeId = (long)PenaltyTypeEnum.LostBook,
                        Description = "Book lost claim, lost book penalty is not included yet",
                        Amount = amount,
                        OverDueDays = overdueDays,
                    };
                }
                else
                {
                    penaltyOfTransection.Amount += amount;
                    penaltyOfTransection.OverDueDays = overdueDays;
                }

                penaltyToUpdate.Add(penaltyOfTransection);
                x.Penalties = null;
            });
        }

        if (transectionToUpdate.Any())
            _repositoryManager.TransectionRepository.UpdateRange(entities: transectionToUpdate);

        if (usersWithPenaltyInfo.Any() || expiredTransection.Any())
        {
            _repositoryManager.PenaltyRepository.UpdateRange(entities: penaltyToUpdate);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
        }
    }

    #region Private section
    private async Task<(int previousLimitCarryoverDays, int membershipExpiryBufferDays)> GetBufferTimeFromConfig()
    {
        var bufferTime = await _repositoryManager.ConfigRepository
            .GetByKeyNameListAsync(new List<string>() { "PreviousLimitCarryoverDays", "MembershipExpiryBufferDays" })
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        int.TryParse(bufferTime
            .FirstOrDefault(x => x.KeyName.Equals("PreviousLimitCarryoverDays", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int previousLimitCarryoverDays);

        int.TryParse(bufferTime
            .FirstOrDefault(x => x.KeyName.Equals("MembershipExpiryBufferDays", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int membershipExpiryBufferDays);

        return (previousLimitCarryoverDays, membershipExpiryBufferDays);
    }

    private async Task<long> GetBorrowDueDays()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync("BorrowDueDays").FirstOrDefaultAsync())?.KeyValue ?? "0", out long borrowDueDays);
        return borrowDueDays;
    }

    private async Task<(int basePenaltyPerDay, int increaseValue, int intervalDays, string increaseType)> GetPenaltyInfoFromConfig()
    {
        var penaltyInfo = await _repositoryManager.ConfigRepository
            .GetByKeyNameListAsync(new List<string>() { "BasePenaltyPerDay", "PenaltyIncreaseType", "PenaltyIncreaseValue", "PenaltyIncreaseDurationInDays" })
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        int.TryParse(penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals("BasePenaltyPerDay", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int basePenaltyPerDay);

        int.TryParse(penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals("PenaltyIncreaseValue", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int increaseValue);

        int.TryParse(penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals("PenaltyIncreaseDurationInDays", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int intervalDays);

        var increaseType = penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals("PenaltyIncreaseType", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "+";

        return (basePenaltyPerDay, increaseValue, intervalDays, increaseType);
    }

    private decimal CalculateDynamicPenalty(int overdueDays, int lastCalculatedDay, int basePenalty, string increaseType, int increaseValue, int intervalDays, int penalizedItemCount = 1)
    {
        decimal totalPenalty = 0;

        for (int i = lastCalculatedDay + 1; i <= overdueDays; i++)
        {
            int interval = (i / intervalDays);
            int penaltyRate = basePenalty;

            if (interval > 0)
            {
                penaltyRate = increaseType switch
                {
                    "+" => penaltyRate + (interval * increaseValue),
                    "*" => penaltyRate * (int)Math.Pow(increaseValue, interval),
                    _ => penaltyRate
                };

                if (increaseType == "ADD")
                    penaltyRate += (interval * increaseValue);
                else if (increaseType == "MULTIPLY")
                    penaltyRate *= (int)Math.Pow(increaseValue, interval);
            }

            totalPenalty += penaltyRate * penalizedItemCount;
        }

        return totalPenalty;
    }
    #endregion
}
