using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Application.Services.Constants;
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

public class PenaltyService : IPenaltyService
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

        penaltyQuery = penaltyQuery.Where(x => StatusGroups.FineStatus.Settled.Contains(x.StatusId));

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

        if (penalty.TransectionId != null && existPenalty.StatusId == (long)FineStatusEnum.UnPaid && StatusGroups.FineStatus.Settled.Contains(penalty.StatusId))
        {
            if (!StatusGroups.Transaction.Finalized.Contains(penalty.TransectionStatusId ?? 0))
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

    /// <summary>
    /// Calculates and updates penalties for various scenarios in the library system.
    /// This method handles 4 types of penalties:
    /// 1. Extra holdings with active membership (user holds more books than plan allows)
    /// 2. Books held under expired membership (user's membership expired but still has books)
    /// 3. Overdue transactions (books not returned/renewed by due date)
    /// 4. Lost book claims (user claimed book as lost)
    /// 
    /// EXECUTION CONTEXT:
    /// - Primary: Scheduled Hangfire job (runs daily in morning)
    /// - Secondary: Called in real-time during:
    ///   * Get book info by ID (prevents borrowing if penalties exist)
    ///   * Borrow book API (ensures no stale penalty data)
    ///   * Get transactions (shows latest overdue status)
    /// 
    /// PERFORMANCE CONSIDERATIONS:
    /// - Uses minimal DB queries (2-3 total) for efficiency
    /// - Processes all penalty types in single method to avoid multiple DB round trips
    /// - Uses AsEnumerable() only when SQL translation is not possible
    /// 
    /// IDEMPOTENCY:
    /// - Safe to call multiple times
    /// - Only updates penalties that have changed (different OverDueDays)
    /// - Incremental calculation (doesn't recalculate from zero)
    /// </summary>
    /// <param name="forLogin">
    /// If true, calculates penalties only for the currently authenticated user.
    /// If false, calculates penalties for all users in the system.
    /// Used when called from user-specific operations vs. scheduled job.
    /// </param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task CalculatePenaltyForHoldingBooks(bool forLogin = false)
    {
        // Get authenticated user ID if calculating for specific user
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        // Retrieve configuration values for buffer periods
        (int previousLimitCarryoverDays, int membershipExpiryBufferDays) = await GetBufferTimeFromConfig();

        #region Step 1: Identify Users with Over-Holding (More books than allowed)

        // Query users who have more active transactions than their membership allows
        // NOTE: Must use AsEnumerable() because ActiveTransactionCount is calculated in-memory
        // and cannot be compared with BorrowLimit in SQL
        var usersWithOverHolding = _repositoryManager.UserRepository
            .FindByCondition(x => x.IsActive && (!forLogin || x.Id == authUserId))
            .Select(x => new
            {
                UserId = x.Id,
                // Count only active transactions (not returned, cancelled, or claimed lost)
                ActiveTransectionCount = x.Transections.Count(t =>
                    t.IsActive &&
                    !StatusGroups.Transaction.Finalized.Contains(t.StatusId)
                ),
                // Get current active membership if exists
                ActiveMembership = x.UserMemberships
                    .Where(m =>
                        m.IsActive &&
                        m.EffectiveStartDate < DateTimeOffset.UtcNow &&
                        m.ExpirationDate > DateTimeOffset.UtcNow
                    )
                    .FirstOrDefault(),
                // Get existing unpaid holding-related penalties
                Penalties = x.Penalties.Where(p =>
                    p.IsActive &&
                    p.StatusId == (long)FineStatusEnum.UnPaid &&
                    StatusGroups.PenaltyType.HoldingRelated.Contains(p.PenaltyTypeId)
                ),
                // Get last expired membership date for buffer calculation
                LastExpireMembershipDate = x.UserMemberships
                    .Where(m => m.ExpirationDate <= DateTimeOffset.UtcNow)
                    .OrderByDescending(m => m.ExpirationDate)
                    .Select(m => m.ExpirationDate)
                    .FirstOrDefault(),
            })
            .AsEnumerable() // Switch to client-side evaluation for calculated property comparison
            .Where(x => x.ActiveTransectionCount > (x.ActiveMembership?.BorrowLimit ?? 0))
            .ToList();

        #endregion

        #region Step 2: Filter Users with Active Membership Over-Holding

        // Users who have active membership but are holding more books than allowed
        // Penalty applies only after carryover period from previous membership expires
        var overHoldingWithActiveMembership = usersWithOverHolding
            .Where(x => x.ActiveMembership != null
                && (DateTimeOffset.UtcNow.Date - x.ActiveMembership.EffectiveStartDate.Date).Days > previousLimitCarryoverDays
                && (x.Penalties == null
                    || !x.Penalties.Any()
                    || x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.ActiveMembership.EffectiveStartDate.Date).Days - previousLimitCarryoverDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.ExtraHoldings)))
            .ToList();

        #endregion

        #region Step 3: Filter Users with Expired Membership Holding Books

        // Users whose membership has expired but still holding books
        // Penalty applies only after buffer period from expiration date
        var overHoldingWithExpiredMembership = usersWithOverHolding
            .Where(x => x.ActiveMembership == null
                && x.LastExpireMembershipDate != default
                && (DateTimeOffset.UtcNow.Date - x.LastExpireMembershipDate.Date).Days > membershipExpiryBufferDays
                && (x.Penalties == null ||
                    !x.Penalties.Any() ||
                    x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.LastExpireMembershipDate.Date).Days - membershipExpiryBufferDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.BooksHeldUnderExpiredMembership)))
            .ToList();

        #endregion

        #region Step 4: Log Users with No Membership History

        // Edge case: Users holding books but no membership data found
        // This indicates data inconsistency that needs investigation
        var usersWithNoMembershipEver = usersWithOverHolding
            .Where(x => x.ActiveMembership == null && x.LastExpireMembershipDate == default)
            .Select(x => x.UserId)
            .ToList();

        if (usersWithNoMembershipEver.Any())
        {
            _logger.LogWarning($"{String.Join(",", usersWithNoMembershipEver)} users have book holding but haven't any purchase membership data found for them");
        }

        #endregion

        #region Step 5: Build Penalty Calculation Info for Over-Holding Users

        // Combine both active and expired membership over-holdings into single list
        // with calculated overdue days and descriptions
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

        #endregion

        #region Step 6: Get Overdue Transactions (Past Due Date)

        // Find all transactions that are past due date and not returned/cancelled/lost
        // NOTE: AsEnumerable() needed because we compare calculated OverdueDays with stored value
        var expiredTransection = await _repositoryManager.TransectionRepository
            .FindByCondition(x => x.IsActive
                && (forLogin == false || forLogin == true && x.Id == authUserId)
                && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)
                && x.DueDate.Date < DateTimeOffset.UtcNow.Date)
            .Include(x => x.Penalties.Where(x => x.IsActive && x.PenaltyTypeId == (long)PenaltyTypeEnum.LateReturnRenew && x.StatusId == (long)FineStatusEnum.UnPaid))
            .ToListAsync(); // Switch to client-side for calculated OverdueDays comparison

        expiredTransection = expiredTransection
            .Where(x => x.Penalties == null
                || !x.Penalties.Any()
                || x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date - x.DueDate.Date).Days
                    && p.StatusId == (long)FineStatusEnum.UnPaid
                    && p.PenaltyTypeId == (long)PenaltyTypeEnum.LateReturnRenew))
            .ToList();

        #endregion

        #region Step 7: Get Claimed Lost Transactions Needing Penalty Update

        // Find transactions marked as lost with penalties that need updating
        // NOTE: Penalty always exists for ClaimedLost (created when claim is made)
        // We only need to update if OverdueDays has changed
        var pendingLostClaimPenalty = await _repositoryManager.TransectionRepository
            .FindByCondition(x => x.IsActive
                && (forLogin == false || forLogin == true && x.Id == authUserId)
                && x.StatusId == (long)TransectionStatusEnum.ClaimedLost
                && x.LostClaimDate.HasValue)
            .Include(x => x.Penalties.Where(x => x.IsActive && x.PenaltyTypeId == (long)PenaltyTypeEnum.LostBook && x.StatusId == (long)FineStatusEnum.UnPaid))
            .ToListAsync();

        pendingLostClaimPenalty = pendingLostClaimPenalty
            .Where(x => x.Penalties.Any(p => p.OverDueDays != (DateTimeOffset.UtcNow.Date -           x.LostClaimDate!.Value.Date).Days
                && p.StatusId == (long)FineStatusEnum.UnPaid
                && p.PenaltyTypeId == (long)PenaltyTypeEnum.LostBook))
            .ToList();

        #endregion

        #region Step 8: Initialize Collections for Updates

        List<Penalty> penaltyToUpdate = new List<Penalty>();
        List<Transection> transectionToUpdate = new List<Transection>();

        #endregion

        #region Step 9: Get Penalty Calculation Configuration

        (int basePenaltyPerDay, int increaseValue, int intervalDays, string increaseType) = await GetPenaltyInfoFromConfig();

        #endregion

        #region Step 10: Process User Over-Holding Penalties

        if (usersWithPenaltyInfo?.Any() == true)
        {
            usersWithPenaltyInfo.ForEach(x =>
            {
                // Find existing penalty for this user and penalty type
                var penaltyOfUser = x.Penalties?.Where(p =>
                        p.StatusId == (long)FineStatusEnum.UnPaid
                        && p.OverDueDays != x.OverdueDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.ExtraHoldings
                    ).FirstOrDefault();

                // Calculate incremental penalty (only for new days since last calculation)
                var amount = CalculateDynamicPenalty(
                    overdueDays: x.OverdueDays, 
                    lastCalculatedDay: penaltyOfUser?.OverDueDays ?? 0, 
                    basePenalty: basePenaltyPerDay, 
                    increaseType: increaseType, 
                    increaseValue: increaseValue, 
                    intervalDays: intervalDays, 
                    penalizedItemCount: x.OverHoldCount
                );

                if (penaltyOfUser is null)
                {
                    // Create new penalty
                    penaltyOfUser = new Penalty()
                    {
                        UserId = x.UserId,
                        TransectionId = null, // User-level penalty, not transaction-specific
                        StatusId = (long)FineStatusEnum.UnPaid,
                        PenaltyTypeId = x.PenaltyType,
                        Description = x.Description,
                        Amount = amount,
                        OverDueDays = x.OverdueDays,
                    };
                }
                else
                {
                    // Update existing penalty (increment amount, update overdue days)
                    penaltyOfUser.Amount += amount;
                    penaltyOfUser.OverDueDays = x.OverdueDays;
                }

                penaltyToUpdate.Add(penaltyOfUser);
            });
        }

        #endregion

        #region Step 11: Process Overdue Transaction Penalties

        if (expiredTransection?.Any() == true)
        {
            expiredTransection.ForEach(x =>
            {
                var overdueDays = (DateTimeOffset.UtcNow.Date - x.DueDate.Date).Days;

                // Find existing overdue penalty for this transaction
                var penaltyOfTransection = x.Penalties?.Where(p =>
                        p.StatusId == (long)FineStatusEnum.UnPaid
                        && p.OverDueDays != overdueDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.LateReturnRenew
                    ).FirstOrDefault();

                // Calculate incremental penalty
                var amount = CalculateDynamicPenalty(
                    overdueDays: overdueDays, 
                    lastCalculatedDay: penaltyOfTransection?.OverDueDays ?? 0, 
                    basePenalty: basePenaltyPerDay, 
                    increaseType: increaseType, 
                    increaseValue: increaseValue, 
                    intervalDays: intervalDays
                );

                if (penaltyOfTransection is null)
                {
                    // Create new overdue penalty
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
                    // Update existing penalty
                    penaltyOfTransection.Amount += amount;
                    penaltyOfTransection.OverDueDays = overdueDays;
                }

                penaltyToUpdate.Add(penaltyOfTransection);

                // Update transaction status to Overdue if not already
                if (x.StatusId != (long)TransectionStatusEnum.Overdue)
                {
                    x.Penalties = null; // Detach navigation to prevent EF tracking issues
                    x.StatusId = (long)TransectionStatusEnum.Overdue;
                    transectionToUpdate.Add(x);
                }
            });
        }

        #endregion

        #region Step 12: Process Lost Book Claim Penalties

        if (pendingLostClaimPenalty?.Any() == true)
        {
            pendingLostClaimPenalty.ForEach(x =>
            {
                var overdueDays = (DateTimeOffset.UtcNow.Date - x.LostClaimDate!.Value.Date).Days;

                // Find existing lost book penalty
                var penaltyOfTransection = x.Penalties?.Where(p =>
                        p.StatusId == (long)FineStatusEnum.UnPaid
                        && p.OverDueDays != overdueDays
                        && p.PenaltyTypeId == (long)PenaltyTypeEnum.LostBook
                    ).FirstOrDefault();

                // Calculate incremental penalty
                var amount = CalculateDynamicPenalty(
                    overdueDays: overdueDays, 
                    lastCalculatedDay: penaltyOfTransection?.OverDueDays ?? 0, 
                    basePenalty: basePenaltyPerDay, 
                    increaseType: increaseType, 
                    increaseValue: increaseValue, 
                    intervalDays: intervalDays
                );

                if (penaltyOfTransection is null)
                {
                    // Edge case: ClaimedLost transaction should always have penalty
                    // Log warning about data inconsistency
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
                    // Update existing penalty
                    penaltyOfTransection.Amount += amount;
                    penaltyOfTransection.OverDueDays = overdueDays;
                }

                penaltyToUpdate.Add(penaltyOfTransection);
                x.Penalties = null; // Detach navigation
            });
        }

        #endregion

        #region Step 13: Save All Updates

        // Update transaction statuses first (if any)
        if (transectionToUpdate.Any())
            _repositoryManager.TransectionRepository.UpdateRange(entities: transectionToUpdate);

        // Update all penalties in single batch operation
        if (usersWithPenaltyInfo?.Any() == true || expiredTransection?.Any() == true)
        {
            _repositoryManager.PenaltyRepository.UpdateRange(entities: penaltyToUpdate);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();
        }

        #endregion
    }

    #region Private section

    /// <summary>
    /// Retrieves buffer time configuration from database.
    /// Buffer times allow grace periods before penalties are applied.
    /// </summary>
    /// <returns>
    /// Tuple containing:
    /// - previousLimitCarryoverDays: Days to carry over previous membership's borrow limit
    /// - membershipExpiryBufferDays: Days after membership expiry before penalty applies
    /// </returns>
    private async Task<(int previousLimitCarryoverDays, int membershipExpiryBufferDays)> GetBufferTimeFromConfig()
    {
        var bufferTime = await _repositoryManager.ConfigRepository
            .GetByKeyNameListAsync(ConfigKeysConstants.BufferTimeConfigKeys)
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        int.TryParse(bufferTime
            .FirstOrDefault(x => x.KeyName.Equals(ConfigKeysConstants.PreviousLimitCarryoverDays, StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int previousLimitCarryoverDays);

        int.TryParse(bufferTime
            .FirstOrDefault(x => x.KeyName.Equals(ConfigKeysConstants.MembershipExpiryBufferDays, StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int membershipExpiryBufferDays);

        return (previousLimitCarryoverDays, membershipExpiryBufferDays);
    }

    /// <summary>
    /// Retrieves borrow duration in days configuration from database.
    /// </summary>
    /// <returns>
    /// Days to carry resource from borrow date
    /// </returns>
    private async Task<long> GetBorrowDueDays()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync(ConfigKeysConstants.BorrowDueDays).FirstOrDefaultAsync())?.KeyValue ?? "0", out long borrowDueDays);
        return borrowDueDays;
    }

    /// <summary>
    /// Retrieves penalty calculation configuration from database.
    /// Defines how penalties increase over time.
    /// </summary>
    /// <returns>
    /// Tuple containing:
    /// - basePenaltyPerDay: Base penalty amount per day
    /// - increaseValue: Amount/factor to increase penalty by
    /// - intervalDays: Days interval for penalty increase
    /// - increaseType: Type of increase (ADD/MULTIPLY or +/*)
    /// </returns>
    private async Task<(int basePenaltyPerDay, int increaseValue, int intervalDays, string increaseType)> GetPenaltyInfoFromConfig()
    {
        var penaltyInfo = await _repositoryManager.ConfigRepository
            .GetByKeyNameListAsync(ConfigKeysConstants.PenaltyConfigKeys)
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        int.TryParse(penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals(ConfigKeysConstants.BasePenaltyPerDay, StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int basePenaltyPerDay);

        int.TryParse(penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals(ConfigKeysConstants.PenaltyIncreaseValue, StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int increaseValue);

        int.TryParse(penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals(ConfigKeysConstants.PenaltyIncreaseDurationInDays, StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "0", out int intervalDays);

        var increaseType = penaltyInfo
            .FirstOrDefault(x => x.KeyName.Equals(ConfigKeysConstants.PenaltyIncreaseType, StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? "+";

        return (basePenaltyPerDay, increaseValue, intervalDays, increaseType);
    }

    /// <summary>
    /// Calculates penalty amount dynamically based on overdue duration and increase method.
    /// Supports both additive and multiplicative penalty increases over time.
    /// 
    /// CALCULATION METHOD:
    /// - For each day from (lastCalculatedDay + 1) to overdueDays:
    ///   1. Determine which interval the day falls into (day / intervalDays)
    ///   2. Calculate penalty rate based on interval and increase type
    ///   3. Multiply by penalizedItemCount (for multi-item penalties)
    ///   4. Add to total penalty
    /// 
    /// EXAMPLES:
    /// - Base: 5Rs/day, Interval: 5 days, Type: ADD, Value: 5
    ///   Day 1-5: 5Rs/day
    ///   Day 6-10: 10Rs/day (5 + 5)
    ///   Day 11-15: 15Rs/day (5 + 10)
    /// 
    /// - Base: 5Rs/day, Interval: 5 days, Type: MULTIPLY, Value: 2
    ///   Day 1-5: 5Rs/day
    ///   Day 6-10: 10Rs/day (5 * 2^1)
    ///   Day 11-15: 20Rs/day (5 * 2^2)
    /// </summary>
    /// <param name="overdueDays">Total days the item is overdue</param>
    /// <param name="lastCalculatedDay">Last day for which penalty was calculated (for incremental updates)</param>
    /// <param name="basePenalty">Base penalty amount per day</param>
    /// <param name="increaseType">Type of increase: ADD/+ (additive) or MULTIPLY/* (multiplicative)</param>
    /// <param name="increaseValue">Amount to add or multiply by per interval</param>
    /// <param name="intervalDays">Number of days in each interval before penalty increases</param>
    /// <param name="penalizedItemCount">Number of items being penalized (default: 1)</param>
    /// <returns>Total penalty amount for the period</returns>
    private decimal CalculateDynamicPenalty(int overdueDays, int lastCalculatedDay, int basePenalty, string increaseType, int increaseValue, int intervalDays, int penalizedItemCount = 1)
    {
        decimal totalPenalty = 0;

        // Calculate penalty for each day from last calculation to current overdue days
        for (int i = lastCalculatedDay + 1; i <= overdueDays; i++)
        {
            // Determine which interval this day falls into
            int interval = (i / intervalDays);
            int penaltyRate = basePenalty;

            // Apply penalty increase based on interval
            if (interval > 0)
            {
                penaltyRate = increaseType.ToLower() switch
                {
                    "add" or "+" => penaltyRate + (interval * increaseValue),
                    "multiply" or "*" => penaltyRate * (int)Math.Pow(increaseValue, interval),
                    _ => penaltyRate // Unknown type, use base penalty
                };
            }

            // Multiply by item count (for penalties on multiple items)
            totalPenalty += penaltyRate * penalizedItemCount;
        }

        return totalPenalty;
    }
    #endregion
}
