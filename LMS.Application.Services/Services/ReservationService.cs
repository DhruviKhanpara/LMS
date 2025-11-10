using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Reservation;
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
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class ReservationService : IReservationService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly HttpContext? _httpContext;
    private readonly ILogger<ReservationService> _logger;
    private readonly INotificationService _notificationService;

    public ReservationService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService, ILogger<ReservationService> logger, INotificationService notificationService)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _validationService = validationService;
        _httpContext = httpContextAccessor.HttpContext;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<PaginatedResponseDto<T>> GetAllReservationAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null, long? bookId = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var reservationQuery = _repositoryManager.ReservationRepository
            .GetReservationOrderByActiveStatus(isActive: authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true)
            .Where(x => (!userId.HasValue || x.UserId == (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) ? authUserId : userId)) && (!bookId.HasValue || x.BookId == bookId));

        var totalCount = await reservationQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var reservations = await reservationQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = reservations
        };
    }

    public async Task<PaginatedResponseDto<GetUserReservationDto>> GetUserReservationAsync(int? pageSize = null, int? pageNumber = null)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var reservationQuery = _repositoryManager.ReservationRepository
            .GetReservationOrderByActiveStatus(isActive: true)
            .Where(x => x.UserId == authUserId);

        var activeReservation = await reservationQuery
            .Where(x => !new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(x.StatusId))
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<GetUserReservationDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        reservationQuery = reservationQuery.Where(x => new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(x.StatusId));

        var totalCount = await reservationQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var reservations = await reservationQuery
            .OrderByDescending(x => x.CreatedAt)
            .ProjectTo<GetUserReservationDto>(_mapper.ConfigurationProvider)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        var transferLimit = await GetTransferLimit();

        activeReservation.ForEach(x =>
        {
            if (x.TransferAllocationCount >= transferLimit)
                x.CanTransferAllocation = false;
            else
                x.CanTransferAllocation = true;
        });

        return new PaginatedResponseDto<GetUserReservationDto>()
        {
            Pagination = paginationModel,
            Data = activeReservation,
            Data1 = reservations
        };
    }

    public async Task<T> GetReservationByIdAsync<T>(long id) where T : class
    {
        var reservation = await _repositoryManager.ReservationRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return reservation ?? Activator.CreateInstance<T>();
    }

    public async Task<byte[]> ExportReservationData()
    {
        var transections = await GetAllReservationAsync<ExportReservationDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Reservation", transections.Data } });
    }

    public async Task ReserveBookforLoginUserAsync(long bookId)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || !authUserRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("It is necessary for you to log in as user");

        await ValidateUserAvailability(userId: authUserId);
        await ValidateBookAvailability(bookId: bookId, checkStatus: true);
        await ValidateUserEligibility(userId: authUserId);

        if (await HasUserAlreadyReservedBook(userId: authUserId, bookId: bookId))
            throw new BadRequestException("This book is already reserved by you");

        if (await HasUserAlreadyBorrowedBook(userId: authUserId, bookId: bookId))
            throw new BadRequestException("This book is already borrowed by you");

        var reservation = new Reservation();
        reservation.Id = 0;
        reservation.UserId = authUserId;
        reservation.BookId = bookId;
        reservation.StatusId = (long)ReservationsStatusEnum.Reserved;
        reservation.AllocateAfter = DateTimeOffset.UtcNow;
        reservation.ReservationDate = DateTimeOffset.UtcNow;
        reservation.IsAllocated = false;
        reservation.IsActive = true;

        await _repositoryManager.ReservationRepository.AddAsync(reservation);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task AddReservationAsync(AddReservationDto reservation)
    {
        _validationService.Validate<AddReservationDto>(reservation);

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || authUserRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("As a User you can't add reservation through this");

        if (!Enum.IsDefined(typeof(ReservationsStatusEnum), reservation.StatusId))
            throw new BadRequestException("Undefined Reservation status");

        if (reservation.AllocateAfter is null || reservation.AllocateAfter < reservation.ReservationDate)
            throw new BadRequestException("Invalid AllocateAfter date");

        await ValidateUserAvailability(userId: reservation.UserId);

        bool checkStatus = !new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(reservation.StatusId);

        await ValidateBookAvailability(bookId: reservation.BookId, checkStatus: checkStatus);

        if (checkStatus)
        {
            await ValidateUserEligibility(userId: reservation.UserId);

            if (await HasUserAlreadyReservedBook(userId: reservation.UserId, bookId: reservation.BookId))
                throw new BadRequestException("This book is already reserved by you");

            if (await HasUserAlreadyBorrowedBook(userId: reservation.UserId, bookId: reservation.BookId))
                throw new BadRequestException("This book is already borrowed by you");

            if (reservation.IsAllocated || reservation.AllocatedAt is not null)
            {
                _logger.LogWarning($"Request Interrupted: While adding reservation Allocation Date or Allocation flag is detected.");
                reservation.IsAllocated = false;
                reservation.AllocatedAt = null;
            }
        }
        else
        {
            var anyPastUserMembership = await _repositoryManager.UserMembershipMappingRepository
                .FindByCondition(x => x.UserId == reservation.UserId)
                .AnyAsync();

            if (!anyPastUserMembership)
                throw new BadRequestException("This user haven't any memberhsip plan till now");
        }

        if (reservation.StatusId != (long)ReservationsStatusEnum.Cancelled && reservation.CancelReason is not null)
        {
            _logger.LogWarning($"Request Interrupted: CancelReason is detected while status is not Cancel while adding reservation.");
            reservation.CancelReason = null;
        }

        reservation.TransferAllocationCount = 0;
        var mappedReservation = _mapper.Map<Reservation>(reservation);
        mappedReservation.Id = 0;

        await _repositoryManager.ReservationRepository.AddAsync(mappedReservation);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdateReservationAsync(UpdateReservationDto reservation)
    {
        _validationService.Validate<UpdateReservationDto>(reservation);

        if (!Enum.IsDefined(typeof(ReservationsStatusEnum), reservation.StatusId))
            throw new BadRequestException("Undefined Reservation status");

        var existReservation = await _repositoryManager.ReservationRepository.GetAllAsync(isActive: true).Where(x => x.Id == reservation.Id).FirstOrDefaultAsync();

        if (existReservation == null)
            throw new BadRequestException("This Reservation is not available");

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || (authUserRole.ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase) && existReservation.UserId != authUserId))
            throw new BadRequestException("As a User you can't update this reservation");

        if (reservation.AllocateAfter is null || reservation.AllocateAfter < reservation.ReservationDate)
            throw new BadRequestException("Invalid AllocateAfter date");

        await ValidateUserAvailability(userId: reservation.UserId);

        bool checkStatus = new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(existReservation.StatusId)
            && !new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(reservation.StatusId);

        await ValidateBookAvailability(bookId: reservation.BookId, checkStatus: checkStatus);

        if (checkStatus)
        {
            await ValidateUserEligibility(userId: reservation.UserId);

            if (await HasUserAlreadyReservedBook(userId: reservation.UserId, bookId: reservation.BookId, id: reservation.Id))
                throw new BadRequestException("This book is already reserved by you");

            if (await HasUserAlreadyBorrowedBook(userId: reservation.UserId, bookId: reservation.BookId))
                throw new BadRequestException("This book is already borrowed by you");
        }

        if (existReservation.IsAllocated != true && reservation.IsAllocated == true)
        {
            _logger.LogWarning($"Request Interrupted: Request have Allocation flag true while existing reservation havn't allocation.");
            reservation.IsAllocated = false;
        }

        if (existReservation.StatusId != (long)ReservationsStatusEnum.Allocated && reservation.StatusId == (long)ReservationsStatusEnum.Allocated)
        {
            _logger.LogWarning($"Request Interrupted: Request have Allocated status while exist reservation haven't allocation.");
            reservation.StatusId = existReservation.StatusId;
        }

        if (reservation.StatusId != (long)ReservationsStatusEnum.Cancelled && reservation.CancelReason is not null)
        {
            _logger.LogWarning($"Request Interrupted: CancelReason is detected while status is not Cancel while updating reservation.");
            reservation.CancelReason = null;
        }

        bool IsAllocationStatusUpdated = existReservation.StatusId == (long)ReservationsStatusEnum.Allocated && existReservation.IsAllocated
            && reservation.StatusId != (long)ReservationsStatusEnum.Allocated && !reservation.IsAllocated;

        if (!reservation.IsCountTransferAllocation && authUserRole.Equals(nameof(RoleListEnum.Librarian), StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogWarning($"Request Interrupted: IsCountTransferAllocation flag found false with librarian request while update reservation.");
            reservation.IsCountTransferAllocation = true;
        }

        if (reservation.IsCountTransferAllocation && reservation.TransferAllocationCount != existReservation.TransferAllocationCount)
        {
            _logger.LogWarning($"Request Interrupted: Transfer allocation count found modified with IsCountTransferAllocation flag true while update reservation.");
            reservation.TransferAllocationCount = existReservation.TransferAllocationCount;
        }

        if (IsAllocationStatusUpdated)
        {
            if (reservation.IsAllocateAfterIsDefault)
                reservation.AllocateAfter = DateTimeOffset.UtcNow.AddDays(await GetDefaultAllocationDelayDays());

            if (reservation.IsCountTransferAllocation)
                reservation.TransferAllocationCount = existReservation.TransferAllocationCount + 1;
        }

        if (reservation.TransferAllocationCount > await GetTransferLimit())
            throw new BadRequestException("User's allocation transfer limit is over not can't transfer the allocation need to borrow or cancel");

        var mappedReservation = _mapper.Map(reservation, existReservation);

        if (IsAllocationStatusUpdated)
        {
            await MoveFreeBookToRack(bookId: mappedReservation.BookId);
        }

        _repositoryManager.ReservationRepository.Update(mappedReservation);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task RemoveBookReservationAsync(long bookId)
    {
        var existBookReservations = await _repositoryManager.ReservationRepository.FindByCondition(x => x.BookId == bookId && x.IsActive).ToListAsync();

        if (existBookReservations.Any())
        {
            await DeteleReservation(existBookReservations);
        }
    }

    public async Task RemoveUserReservationAsync(long userId)
    {
        var existUserReservations = await _repositoryManager.ReservationRepository.FindByCondition(x => x.UserId == userId && x.IsActive).ToListAsync();

        if (existUserReservations.Any())
        {
            await DeteleReservation(existUserReservations);
        }
    }

    public async Task ReservationActionsAsync(long id, ReservationActionEnum reservationAction)
    {
        var existBookReservation = await _repositoryManager.ReservationRepository.GetAllAsync(isActive: true).Where(x => x.Id == id).FirstOrDefaultAsync();

        if (existBookReservation == null)
            throw new BadRequestException("This Book reservation is already not exist");

        long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        if (_httpContext!.GetUserRole().ToLower().Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase) && existBookReservation.UserId != authUserId)
            throw new BadRequestException("you can't perform any actions on other's reservation as a user");

        bool hasAllocation = existBookReservation.IsAllocated
            && existBookReservation.StatusId == (long)ReservationsStatusEnum.Allocated
            && new[] { ReservationActionEnum.Cancel, ReservationActionEnum.Delete, ReservationActionEnum.Transfer }.Contains(reservationAction);

        switch (reservationAction)
        {
            case ReservationActionEnum.Cancel:
                existBookReservation.StatusId = (long)ReservationsStatusEnum.Cancelled;
                existBookReservation.CancelReason = "Cancle By the User";
                break;

            case ReservationActionEnum.Transfer:
                if (existBookReservation.StatusId == (long)ReservationsStatusEnum.Allocated)
                {
                    if (existBookReservation.TransferAllocationCount >= await GetTransferLimit())
                        throw new BadRequestException("Transfer limit is over");

                    existBookReservation.AllocateAfter = DateTimeOffset.UtcNow.AddDays(await GetDefaultAllocationDelayDays());
                    existBookReservation.StatusId = (long)ReservationsStatusEnum.Reserved;
                    existBookReservation.IsAllocated = false;
                    existBookReservation.TransferAllocationCount++;
                }
                break;

            case ReservationActionEnum.Delete:
                existBookReservation.IsActive = false;
                break;
        }

        if (hasAllocation)
        {
            await MoveFreeBookToRack(bookId: existBookReservation.BookId);
        }

        _repositoryManager.ReservationRepository.Update(entity: existBookReservation);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task PermanentDeleteReservationAsync(long id)
    {
        var existBookReservation = await _repositoryManager.ReservationRepository.GetAllAsync(isActive: false).Where(x => x.Id == id).FirstOrDefaultAsync();

        if (existBookReservation == null)
            throw new BadRequestException("This reservation is already not exist");

        _repositoryManager.ReservationRepository.Remove(entity: existBookReservation);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task ReallocateExpiredAllocationToReservationAsync(bool forLogin = false, bool notifyUser = false)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var reservations = await _repositoryManager.ReservationRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.AllocateAfter <= DateTimeOffset.UtcNow && x.StatusId != (long)ReservationsStatusEnum.Cancelled)
            .ToListAsync();

        long allocationDueDays = await GetAllocationDueDays();

        var expiredAllocatedReservation = reservations
            .Where(x => x.IsAllocated
                && x.StatusId == (long)ReservationsStatusEnum.Allocated
                && (forLogin == false || forLogin == true && x.UserId == authUserId)
                && (x.AllocatedAt.HasValue && x.AllocatedAt.Value.AddDays(allocationDueDays).Date < DateTime.UtcNow.Date))
            .ToList();

        if (!expiredAllocatedReservation.IsNullOrEmpty() && expiredAllocatedReservation.Any())
        {
            List<Reservation> modifiedReservations = new List<Reservation>();

            var books = await _repositoryManager.BooksRepository.GetAllAsync()
                .Where(x => expiredAllocatedReservation.Select(x => x.BookId).Contains(x.Id))
                .ToListAsync();
            var bookDictionary = books.ToDictionary(b => b.Id);

            foreach (var reservation in expiredAllocatedReservation)
            {
                reservation.StatusId = (long)ReservationsStatusEnum.Cancelled;
                reservation.CancelReason = "Timeout";
                modifiedReservations.Add(reservation);

                if (bookDictionary.TryGetValue(reservation.BookId, out var book))
                {
                    book.AvailableCopies++;
                    if (book.TotalCopies < book.AvailableCopies)
                    {
                        _logger.LogWarning($"BookId: {book.Id} detect with more availabel copies({book.AvailableCopies}) then total copies({book.TotalCopies}) while update the availabel copies for expire reservation allocation.");
                        throw new BadRequestException($"Book: {book.Title} detect with more availabel copies then total copies");
                    }
                }
            }

            var eligibleBooks = bookDictionary.Values.Where(b => b.IsActive && new[] { (long)BookStatusEnum.Available, (long)BookStatusEnum.Reserved }.Contains(b.StatusId)).ToDictionary(b => b.Id);

            foreach (var book in eligibleBooks.Values)
            {
                var reservationIdsToAllocate = reservations.Where(x => x.BookId == book.Id
                        && x.IsActive
                        && !x.IsAllocated
                        && x.AllocateAfter <= DateTimeOffset.UtcNow
                        && !new[] { (long)ReservationsStatusEnum.Fulfilled, (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Allocated }.Contains(x.StatusId))
                    .OrderBy(x => x.ReservationDate)
                    .Take((int)book.AvailableCopies)
                    .Select(x => x.Id)
                    .ToList();

                foreach (var reservation in reservations.Where(r => reservationIdsToAllocate.Contains(r.Id)))
                {
                    reservation.IsAllocated = true;
                    reservation.AllocatedAt = DateTimeOffset.UtcNow;
                    reservation.StatusId = (long)ReservationsStatusEnum.Allocated;
                    modifiedReservations.Add(reservation);
                }

                book.AvailableCopies -= reservationIdsToAllocate.Count;

                book.StatusId = book.AvailableCopies == 0 ? (long)BookStatusEnum.Reserved : (long)BookStatusEnum.Available;
            }

            _repositoryManager.BooksRepository.UpdateRange(entities: books);
            _repositoryManager.ReservationRepository.UpdateRange(entities: modifiedReservations);
            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            var newAllocations = modifiedReservations.Where(x => x.StatusId == (long)ReservationsStatusEnum.Allocated).ToList();

            if (newAllocations.Any() && notifyUser)
                await DispatchEmailForReservationAllocation(ids: newAllocations.Select(x => x.Id).Distinct().ToList());
        }
    }

    public async Task AllocateBookToReservation(bool forLogin = false, bool notifyUser = false)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        // Only allocate to login user if their reservation is early enough in the queue
        var reservations = await _repositoryManager.ReservationRepository
            .GetAllAsync(true)
            .Where(x => x.IsActive
                && !x.IsAllocated
                && x.AllocateAfter <= DateTimeOffset.UtcNow
                && !new[] { (long)ReservationsStatusEnum.Fulfilled, (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Allocated }.Contains(x.StatusId))
            .Where(x => x.Book.StatusId == (long)BookStatusEnum.Available)
            .ToListAsync();

        if (reservations.Any())
        {
            var bookIds = reservations
                .Where(x => forLogin == false || (forLogin == true && x.UserId == authUserId))
                .Select(x => x.BookId).Distinct().ToList();

            var books = await _repositoryManager.BooksRepository.GetAllAsync(isActive: true)
                .Where(x => bookIds.Contains(x.Id) && x.StatusId == (long)BookStatusEnum.Available)
                .ToListAsync();

            var bookDictionary = books.ToDictionary(b => b.Id);

            List<Reservation> reservationToAllocate = new List<Reservation>();

            foreach (var book in books)
            {
                var selectedReservations = reservations
                    .Where(x => x.BookId == book.Id)
                    .OrderBy(x => x.ReservationDate)
                    .Take((int)book.AvailableCopies)
                    .Where(x => forLogin == false || (forLogin == true && x.UserId == authUserId))
                    .ToList();

                foreach (var reservation in selectedReservations)
                {
                    reservation.IsAllocated = true;
                    reservation.AllocatedAt = DateTimeOffset.UtcNow;
                    reservation.StatusId = (long)ReservationsStatusEnum.Allocated;
                }
                reservationToAllocate.AddRange(selectedReservations);

                book.AvailableCopies -= selectedReservations.Count;
                book.StatusId = book.AvailableCopies == 0 ? (long)BookStatusEnum.Reserved : (long)BookStatusEnum.Available;
            }

            if (reservationToAllocate.Any())
            {
                _repositoryManager.BooksRepository.UpdateRange(entities: books);
                _repositoryManager.ReservationRepository.UpdateRange(entities: reservationToAllocate);
                await _repositoryManager.UnitOfWork.SaveChangesAsync();

                if (notifyUser)
                    await DispatchEmailForReservationAllocation(ids: reservationToAllocate.Select(x => x.Id).Distinct().ToList());
            }
        }
    }

    public async Task DispatchEmailForReservationAllocation(List<long>? ids = null)
    {
        long allocationDueDays = await GetAllocationDueDays();

        var filteredReservations = await _repositoryManager.ReservationRepository
            .GetAllAsync(isActive: true)
            .Where(x => (ids == null || ids.Contains(x.Id))
                && x.IsAllocated
                && (long)ReservationsStatusEnum.Allocated == x.StatusId
                && x.AllocatedAt.HasValue
                && (x.AllocatedAt.Value.Date == DateTimeOffset.UtcNow.AddDays(allocationDueDays).Date || x.AllocatedAt.Value.Date == DateTimeOffset.UtcNow.Date))
            .Include(x => x.User)
            .Include(x => x.Book)
            .ToListAsync();

        if (filteredReservations.Any())
        {
            var groupedByUser = filteredReservations
            .GroupBy(x => x.UserId)
            .Select(group => group.ToList())
            .Cast<object>()
            .ToList();

            await _notificationService.DispatchMultipleEmail(NotificationTypeEnum.ReservationAllocation, groupedByUser);
        }
    }

    #region Private section
    private async Task ValidateUserAvailability(long userId)
    {
        if (!(await _repositoryManager.UserRepository.AnyAsync(x => x.IsActive && x.Id == userId)))
            throw new BadRequestException("This user is not available");
    }

    private async Task ValidateBookAvailability(long bookId, bool checkStatus)
    {
        if (!(await _repositoryManager.BooksRepository.AnyAsync(x => x.IsActive && x.Id == bookId && (!checkStatus || !new[] { (long)BookStatusEnum.Removed }.Contains(x.StatusId)))))
            throw new BadRequestException("This book is not available");
    }

    private async Task ValidateUserEligibility(long userId)
    {
        var userMembership = await _repositoryManager.UserMembershipMappingRepository
            .FindByCondition(x => x.UserId == userId && x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .ProjectTo<GetUserMembershipDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        var reservationCount = await _repositoryManager.ReservationRepository
            .FindByCondition(x => x.IsActive && x.UserId == userId && !new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(x.StatusId))
            .LongCountAsync();

        if (userMembership is null)
            throw new BadRequestException("User haven't planned for reservation of books");
        else if (reservationCount >= userMembership.BorrowLimit)
            throw new BadRequestException("User's reservation limit is over now, can't reserve books");
    }

    private async Task<bool> HasUserAlreadyReservedBook(long userId, long bookId, long? id = null)
    {
        return await _repositoryManager.ReservationRepository
            .AnyAsync(x => x.IsActive && (id == null || x.Id != id) && x.UserId == userId && x.BookId == bookId && !new[] { (long)ReservationsStatusEnum.Cancelled, (long)ReservationsStatusEnum.Fulfilled }.Contains(x.StatusId));
    }

    private async Task<bool> HasUserAlreadyBorrowedBook(long userId, long bookId)
    {
        return await _repositoryManager.TransectionRepository
            .AnyAsync(x => x.IsActive && x.UserId == userId && x.BookId == bookId && !new[] { (long)TransectionStatusEnum.Cancelled, (long)TransectionStatusEnum.Returned, (long)TransectionStatusEnum.ClaimedLost }.Contains(x.StatusId));
    }

    private async Task MoveFreeBookToRack(long bookId)
    {
        var book = await _repositoryManager.BooksRepository.GetByIdAsync(id: bookId).FirstOrDefaultAsync();

        if (book is not null)
        {
            book.AvailableCopies++;
            if (new[] { (long)BookStatusEnum.Available, (long)BookStatusEnum.Reserved, (long)BookStatusEnum.CheckedOut }.Contains(book.StatusId))
            {
                book.StatusId = (long)BookStatusEnum.Available;
            }
            _repositoryManager.BooksRepository.Update(entity: book);
        }
        else
        {
            _logger.LogWarning($"Request Interrupted: Book define in the request is not found while move it to the rack.");
        }
    }

    private async Task<long> GetDefaultAllocationDelayDays()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync("DefalutAllocationDelayInDays").FirstOrDefaultAsync())?.KeyValue ?? "0", out long defaultAllocationDelayDays);

        return defaultAllocationDelayDays;
    }

    private async Task<long> GetAllocationDueDays()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync("AllocationDueDays").FirstOrDefaultAsync())?.KeyValue ?? "0", out long allocationDueDays);

        return allocationDueDays;
    }

    private async Task<long> GetTransferLimit()
    {
        long.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync("MaxTransferAllocationCount").FirstOrDefaultAsync())?.KeyValue ?? "0", out long transferLimit);
        return transferLimit;
    }

    private async Task DeteleReservation(List<Reservation> reservations)
    {
        var reservationBookIdList = reservations
            .Where(x => x.IsAllocated && x.StatusId == (long)ReservationsStatusEnum.Allocated)
            .Select(x => x.BookId)
            .Distinct()
            .ToList();

        var books = await _repositoryManager.BooksRepository
            .FindByCondition(x => reservationBookIdList.Contains(x.Id))
            .ToListAsync();

        var bookDict = books.ToDictionary(b => b.Id);

        reservations.ForEach(x =>
        {
            x.IsActive = false;

            if (x.IsAllocated)
            {
                if (!bookDict.TryGetValue(x.BookId, out var book))
                    throw new BadRequestException("The book does not exist");

                book.AvailableCopies++;

                if (book.StatusId == (long)BookStatusEnum.Reserved)
                    book.StatusId = (long)BookStatusEnum.Available;
            }
        });

        _repositoryManager.BooksRepository.UpdateRange(entities: bookDict.Values.ToList());
        _repositoryManager.ReservationRepository.UpdateRange(entities: reservations);
        _repositoryManager.UnitOfWork.SaveChanges();
    }
    #endregion
}