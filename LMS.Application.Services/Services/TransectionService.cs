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
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class TransectionService : ITransectionService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly HttpContext? _httpContext;
    private readonly ILogger<TransectionService> _logger;
    private readonly INotificationService _notificationService;

    public TransectionService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService, ILogger<TransectionService> logger, INotificationService notificationService)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _validationService = validationService;
        _httpContext = httpContextAccessor.HttpContext;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<PaginatedResponseDto<T>> GetAllTransectionAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null, long? bookId = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var transectionQuery = _repositoryManager.TransectionRepository
            .GetTransectionOrderByActiveStatus(isActive: authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true)
            .Where(x => (!userId.HasValue || x.UserId == (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) ? authUserId : userId)) && (!bookId.HasValue || x.BookId == bookId));

        var totalCount = await transectionQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var transections = await transectionQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = transections
        };
    }

    public async Task<PaginatedResponseDto<GetUserTransectionDto>> GetUserTransectionAsync(int? pageSize = null, int? pageNumber = null)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var transectionQuery = _repositoryManager.TransectionRepository
            .GetTransectionOrderByActiveStatus(isActive: true)
            .Where(x => x.UserId == authUserId);

        var activeTransection = await transectionQuery
            .Where(x => !new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(x.StatusId))
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<GetUserTransectionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        transectionQuery = transectionQuery.Where(x => new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(x.StatusId));

        var totalCount = await transectionQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var transections = await transectionQuery
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<GetUserTransectionDto>(_mapper.ConfigurationProvider)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        var renewLimit = await GetRenewLimit();

        activeTransection.ForEach(x =>
        {
            if (x.RenewCount >= renewLimit)
                x.CanRenewBook = false;
            else
                x.CanRenewBook = true;
        });

        return new PaginatedResponseDto<GetUserTransectionDto>()
        {
            Pagination = paginationModel,
            Data = activeTransection,
            Data1 = transections
        };
    }

    public async Task<T> GetTransectionByIdAsync<T>(long id) where T : class
    {
        var transection = await _repositoryManager.TransectionRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return transection ?? Activator.CreateInstance<T>();
    }

    public async Task BorrowBookforLoginUserAsync(long bookId, long? userId = null)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();
        bool isUser = authUserRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase);


        if (!isLogin || (!userId.HasValue && !isUser))
            throw new BadRequestException("It is necessary for you to log-in as user");

        await ValidateUserAvailability(userId: isUser ? authUserId : userId ?? 0);
        await ValidateBookAvailability(bookId: bookId, userId: isUser ? authUserId : userId ?? 0, checkStatus: true);
        await ValidateUserEligibility(userId: isUser ? authUserId : userId ?? 0);

        if (await HasUserAlreadyBorrowedBook(userId: isUser ? authUserId : userId ?? 0, bookId: bookId))
            throw new BadRequestException("This book is already borrowed by you");

        var transection = new Transection();
        transection.Id = 0;
        transection.UserId = isUser ? authUserId : userId ?? 0;
        transection.BookId = bookId;
        transection.StatusId = (long)TransectionStatusEnum.Borrowed;
        transection.IsActive = true;
        transection.BorrowDate = DateTimeOffset.UtcNow;
        transection.DueDate = transection.BorrowDate.AddDays(await GetBorrowDueDays());

        await _repositoryManager.TransectionRepository.AddAsync(transection);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();

        var newTransection = await _repositoryManager.TransectionRepository
            .GetByIdAsync(transection.Id)
            .Include(x => x.User)
            .Include(x => x.Status)
            .FirstOrDefaultAsync();

        if (newTransection is not null)
            await _notificationService.DispatchEmail(NotificationTypeEnum.NewCheckout, newTransection);
        else
            _logger.LogWarning($"Just added transection not found in table while tring to send e-mail notification : {transection.Id}");
    }

    public async Task<byte[]> ExportTransectionData()
    {
        var transections = await GetAllTransectionAsync<ExportTransectionDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Check-out's", transections.Data } });
    }

    public async Task AddTransectionAsync(AddTransectionDto transection)
    {
        _validationService.Validate<AddTransectionDto>(transection);

        if (!Enum.IsDefined(typeof(TransectionStatusEnum), transection.StatusId))
            throw new BadRequestException("Undefined Transection status");

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || authUserRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("As a User you can't add borrow through this");

        await ValidateUserAvailability(userId: transection.UserId);

        bool checkStatus = !new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(transection.StatusId);

        await ValidateBookAvailability(bookId: transection.BookId, userId: transection.UserId, checkStatus: checkStatus);
        if (checkStatus)
        {
            await ValidateUserEligibility(userId: transection.UserId);

            if (await HasUserAlreadyBorrowedBook(userId: transection.UserId, bookId: transection.BookId))
                throw new BadRequestException("This book is already borrowed by user");

            if (transection.ReturnDate is not null)
            {
                _logger.LogWarning($"Request Interrupted: ReturnDate is found while transection status is not from the (Return, Cancle, ClaimedLost) while adding transections.");
                transection.ReturnDate = null;
            }
        }
        else
        {
            var anyPastUserMembership = await _repositoryManager.UserMembershipMappingRepository
                .FindByCondition(x => x.UserId == transection.UserId)
                .AnyAsync();

            if (!anyPastUserMembership)
                throw new BadRequestException("This user haven't any memberhsip plan till now");
        }

        if (transection.IsDefaultDueDate)
            transection.DueDate = transection.BorrowDate.AddDays(await GetBorrowDueDays());

        if (transection.DueDate is null || transection.DueDate < transection.BorrowDate)
            throw new BadRequestException("Invalid Due date");

        if (!transection.IsCountRenew && authUserRole.Equals(nameof(RoleListEnum.Librarian), StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogWarning($"Request Interrupted: IsCountRenew flag found false with librarian request while adding transections.");
            transection.IsCountRenew = true;
        }

        if (transection.IsCountRenew)
        {
            if (transection.RenewCount != 0)
                _logger.LogWarning($"Request Interrupted: Renew count is found other than 0 with IsCountRenew flag true while adding transections.");

            if (transection.StatusId == (long)TransectionStatusEnum.Renewed)
                transection.RenewCount = 1;
            else
                transection.RenewCount = 0;
        }

        if (!new[] { (long)TransectionStatusEnum.ClaimedLost }.Contains(transection.StatusId) && transection.LostClaimDate is not null)
        {
            _logger.LogWarning($"Request Interrupted: LostClaimDate is found while transection status is not from the (ClaimedLost) while adding transections.");
            transection.LostClaimDate = null;
        }

        if (!new[] { (long)TransectionStatusEnum.Renewed }.Contains(transection.StatusId) && transection.RenewDate is not null)
        {
            _logger.LogWarning($"Request Interrupted: RenewDate is found while transection status is not from the (Renewed) while adding transections.");
            transection.RenewDate = null;
        }

        if (new[] { (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.Cancelled }.Contains(transection.StatusId) && transection.ReturnDate == null)
        {
            _logger.LogWarning($"Request Interrupted: ReturnDate is not found while transection status is from (Return, cancled) while adding transections.");
            transection.ReturnDate = transection.BorrowDate;
        }

        var mappedTransection = _mapper.Map<Transection>(transection);
        mappedTransection.Id = 0;

        await _repositoryManager.TransectionRepository.AddAsync(mappedTransection);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();

        var newTransection = await _repositoryManager.TransectionRepository
            .GetByIdAsync(mappedTransection.Id)
            .Include(x => x.User)
            .Include(x => x.Status)
            .FirstOrDefaultAsync();

        if (newTransection is not null)
            await _notificationService.DispatchEmail(NotificationTypeEnum.NewCheckout, newTransection);
        else
            _logger.LogWarning($"Just added transection not found in table while tring to send e-mail notification : {mappedTransection.Id}");
    }

    public async Task UpdateTransectionAsync(UpdateTransectionDto transection)
    {
        _validationService.Validate<UpdateTransectionDto>(transection);

        if (transection.StatusId == (long)TransectionStatusEnum.Renewed && !transection.RenewDate.HasValue)
            throw new BadRequestException("Renew Date is required");

        if (!Enum.IsDefined(typeof(TransectionStatusEnum), transection.StatusId))
            throw new BadRequestException("Undefined Transection status");

        var existTransection = await _repositoryManager.TransectionRepository.GetByIdAsync(id: transection.Id).FirstOrDefaultAsync();

        if (existTransection == null)
            throw new BadRequestException("This Transection is not available");

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || (authUserRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase) && existTransection.UserId != authUserId))
            throw new BadRequestException("As a User you can't update this borrow");

        await ValidateUserAvailability(userId: existTransection.UserId);

        bool checkStatus = new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(existTransection.StatusId)
            && !new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(transection.StatusId);

        await ValidateBookAvailability(bookId: existTransection.BookId, userId: existTransection.UserId, checkStatus: checkStatus);

        if (checkStatus)
        {
            await ValidateUserEligibility(userId: existTransection.UserId);

            if (await HasUserAlreadyBorrowedBook(userId: existTransection.UserId, bookId: existTransection.BookId, id: transection.Id))
                throw new BadRequestException("This book is already borrowed by user");
        }

        if (transection.IsDefaultDueDate)
        {
            if (transection.StatusId == (long)TransectionStatusEnum.Renewed)
                transection.DueDate = transection.RenewDate?.AddDays(await GetBorrowDueDays()) ?? DateTimeOffset.UtcNow.AddDays(await GetBorrowDueDays());
            else
                transection.DueDate = transection.BorrowDate.AddDays(await GetBorrowDueDays());
        }

        if (!transection.IsCountRenew && authUserRole.Equals(nameof(RoleListEnum.Librarian), StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogWarning($"Request Interrupted: IsCountRenew flag found false with librarian request while adding transections.");
            transection.IsCountRenew = true;
        }

        if (transection.IsCountRenew)
        {
            if (transection.RenewCount != existTransection.RenewCount)
                _logger.LogWarning($"Request Interrupted: Renew count found modified with IsCountRenew flag true while update transections.");

            transection.RenewCount = existTransection.RenewCount;

            if (existTransection.StatusId != (long)TransectionStatusEnum.Renewed && transection.StatusId == (long)TransectionStatusEnum.Renewed)
                transection.RenewCount = existTransection.RenewCount + 1;
        }

        if (transection.RenewCount > await GetRenewLimit())
            throw new BadRequestException("User's renew limit is over not can't renew the book need to return and re-issue it.");

        if (new[] { (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.Cancelled }.Contains(transection.StatusId) && transection.ReturnDate == null)
            throw new BadRequestException("Return date is required");

        if (transection.DueDate is null || transection.DueDate < transection.BorrowDate)
            throw new BadRequestException("Invalid Due date");

        NotificationTypeEnum emailType = new NotificationTypeEnum();

        if (transection.StatusId == (long)TransectionStatusEnum.Renewed && existTransection.StatusId != (long)TransectionStatusEnum.Renewed)
            emailType = NotificationTypeEnum.RenewCheckout;
        else if (transection.StatusId == (long)TransectionStatusEnum.Overdue && existTransection.StatusId != (long)TransectionStatusEnum.Overdue)
            emailType = NotificationTypeEnum.OverdueCheckout;

        var mappedTransection = _mapper.Map(transection, existTransection);

        _repositoryManager.TransectionRepository.Update(mappedTransection);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();

        if (new[] { NotificationTypeEnum.RenewCheckout, NotificationTypeEnum.OverdueCheckout }.Contains(emailType))
        {
            var newTransection = await _repositoryManager.TransectionRepository
            .GetByIdAsync(mappedTransection.Id)
            .Include(x => x.User)
            .FirstOrDefaultAsync();

            if (newTransection is not null)
                await _notificationService.DispatchEmail(emailType, newTransection);
        }
    }

    public async Task TransectionActionsAsync(long id, TransectionActionEnum transectionAction)
    {
        long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authRole = _httpContext!.GetUserRole();

        if (transectionAction == TransectionActionEnum.Delete && (authRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase) || authRole.ToLower().Equals(nameof(RoleListEnum.Librarian), StringComparison.InvariantCultureIgnoreCase)))
            throw new BadRequestException("Have not permission for process");

        var existBookTransection = await _repositoryManager.TransectionRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existBookTransection == null)
            throw new BadRequestException("This Book transection is already not exist");

        if (authRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase) && existBookTransection.UserId != authUserId)
            throw new BadRequestException("you can't intract with others transection as a user");

        if (!new[] { (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.ClaimedLost }.Contains(existBookTransection.StatusId) && new[] { TransectionActionEnum.Return, TransectionActionEnum.Cancel, TransectionActionEnum.Delete }.Contains(transectionAction))
            throw new BadRequestException("This transection occupied a book first return that");

        var haspPenalty = (await _repositoryManager.TransectionRepository.GetByIdAsync(id).ProjectTo<GetTransectionDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync())?.HasPenalty ?? false;

        if (haspPenalty && transectionAction != TransectionActionEnum.ClaimLost)
            throw new BadRequestException("You can't remove this transection, this have unpaid penalty");

        switch (transectionAction)
        {
            case TransectionActionEnum.Cancel:
                existBookTransection.StatusId = (long)TransectionStatusEnum.Cancelled;
                break;

            case TransectionActionEnum.Return:
                existBookTransection.StatusId = (long)TransectionStatusEnum.Returned;
                existBookTransection.ReturnDate = DateTimeOffset.UtcNow;
                break;

            case TransectionActionEnum.Renew:
                if (existBookTransection.RenewCount >= await GetRenewLimit())
                    throw new BadRequestException("Renew limit is over");

                existBookTransection.StatusId = (long)TransectionStatusEnum.Renewed;
                existBookTransection.RenewDate = DateTimeOffset.UtcNow;
                existBookTransection.DueDate = DateTimeOffset.UtcNow.AddDays(await GetBorrowDueDays());
                existBookTransection.RenewCount++;
                break;

            case TransectionActionEnum.Delete:
                existBookTransection.IsActive = false;
                break;

            case TransectionActionEnum.ClaimLost:
                var bookPrice = (await _repositoryManager.BooksRepository.GetByIdAsync(id: existBookTransection.BookId).FirstOrDefaultAsync())?.Price ?? 0;
                existBookTransection.StatusId = (long)TransectionStatusEnum.ClaimedLost;
                existBookTransection.LostClaimDate = DateTimeOffset.UtcNow;
                var penalty = new Penalty()
                {
                    UserId = existBookTransection.UserId,
                    TransectionId = id,
                    PenaltyTypeId = (long)PenaltyTypeEnum.LostBook,
                    Description = "Borrowed book is lost claim is done by user",
                    StatusId = (long)FineStatusEnum.UnPaid,
                    IsActive = true,
                    Amount = bookPrice,
                    OverDueDays = 1
                };
                await _repositoryManager.PenaltyRepository.AddAsync(penalty);
                break;
        }

        _repositoryManager.TransectionRepository.Update(entity: existBookTransection);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();

        if (transectionAction == TransectionActionEnum.Renew)
        {
            var newTransection = await _repositoryManager.TransectionRepository
            .GetByIdAsync(existBookTransection.Id)
            .Include(x => x.User)
            .FirstOrDefaultAsync();

            await _notificationService.DispatchEmail(NotificationTypeEnum.RenewCheckout, existBookTransection);
        }
    }

    public async Task PermanentDeleteTransectionAsync(long id)
    {
        var existBookTransection = await _repositoryManager.TransectionRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existBookTransection == null)
            throw new BadRequestException("This Book transection is already not exist");

        _repositoryManager.TransectionRepository.Remove(entity: existBookTransection);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task DispatchEmailForDueDateRemainder()
    {
        var filteredTransactions = await _repositoryManager.TransectionRepository
            .GetAllAsync(isActive: true)
            .Where(t => t.DueDate.Date == DateTimeOffset.UtcNow.AddDays(3).Date || t.DueDate.Date == DateTimeOffset.UtcNow.Date)
            .Include(x => x.User)
            .Include(x => x.Book)
            .ToListAsync();

        var groupedByUser = filteredTransactions
            .GroupBy(x => x.UserId)
            .Select(group => group.ToList())
            .Cast<object>()
            .ToList();

        await _notificationService.DispatchMultipleEmail(NotificationTypeEnum.DueDateRemainder, groupedByUser);
    }

    #region Private section
    private async Task ValidateBookAvailability(long bookId, long userId, bool checkStatus)
    {
        if (!(await _repositoryManager.BooksRepository
            .AnyAsync(x => x.IsActive
                && x.Id == bookId
                && (!checkStatus
                    || x.StatusId == (long)BookStatusEnum.Available
                    || (x.StatusId == (long)BookStatusEnum.Reserved
                        && x.Reservations.Any(y => y.IsAllocated && y.StatusId == (long)ReservationsStatusEnum.Allocated && y.UserId == userId)))
                    )))
            throw new BadRequestException("This book is not available");
    }

    private async Task ValidateUserAvailability(long userId)
    {
        if (!(await _repositoryManager.UserRepository.AnyAsync(x => x.IsActive && x.Id == userId)))
            throw new BadRequestException("This user is not available");
    }

    private async Task ValidateUserEligibility(long userId)
    {
        var userMembership = await _repositoryManager.UserMembershipMappingRepository
            .FindByCondition(x => x.UserId == userId && x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .ProjectTo<GetUserMembershipDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        var transactionCount = await _repositoryManager.TransectionRepository
            .FindByCondition(x => x.IsActive && x.UserId == userId && !new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(x.StatusId))
            .LongCountAsync();

        if (userMembership is null)
            throw new BadRequestException("User haven't planned for borrowing books");
        else if (transactionCount >= userMembership.BorrowLimit)
            throw new BadRequestException("User's borrow limit is over now, can't borrow books");
    }

    private async Task<bool> HasUserAlreadyBorrowedBook(long userId, long bookId, long? id = null)
    {
        return await _repositoryManager.TransectionRepository
            .AnyAsync(x => x.IsActive && (id == null || x.Id != id) && x.UserId == userId && x.BookId == bookId && !new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(x.StatusId));
    }

    private async Task<long> GetBorrowDueDays()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync("BorrowDueDays").FirstOrDefaultAsync())?.KeyValue ?? "0", out long borrowDueDays);
        return borrowDueDays;
    }

    private async Task<long> GetRenewLimit()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync("MaxRenewCount").FirstOrDefaultAsync())?.KeyValue ?? "0", out long renewLimit);
        return renewLimit;
    }
    #endregion
}
