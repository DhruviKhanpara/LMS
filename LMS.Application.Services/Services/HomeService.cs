using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs.Home;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services.Services;

internal class HomeService : IHomeService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly HttpContext? _httpContext;
    private readonly IMapper _mapper;

    public HomeService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _repositoryManager = repositoryManager;
        _httpContext = httpContextAccessor.HttpContext;
        _mapper = mapper;
    }

    public async Task<List<ChartData>> GenreVisualization(bool? forLogin = null, int? visualizingMonth = null)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        return await GenreVisualizationData(userId: authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) ? authUserId : forLogin != null && forLogin == true ? authUserId : null, visualizingMonth: visualizingMonth);
    }

    public async Task<UserDashboardDto> UserDashboardDataAync()
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || !authUserRole.Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only User have access");

        var transectionData = _repositoryManager.TransectionRepository.GetTransectionOrderByActiveStatus(isActive: true).Where(x => x.UserId == authUserId);

        var unpaidPenalty = _repositoryManager.PenaltyRepository
            .FindByCondition(x => x.IsActive && x.UserId == authUserId && x.StatusId == (long)FineStatusEnum.UnPaid);

        var activeMembershipDetails = await _repositoryManager.UserMembershipMappingRepository
            .FindByCondition(x => x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .ProjectTo<GetUserMembershipDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        var notifications = transectionData
            .Include(x => x.Book)
            .AsEnumerable()
            .Where(x => x.StatusId == (long)TransectionStatusEnum.Overdue || ((x.DueDate - DateTimeOffset.UtcNow).Days <= 2 && new[] { (long)TransectionStatusEnum.Renewed, (long)TransectionStatusEnum.Borrowed }.Contains(x.StatusId)))
            .Select(x => new NotificationData
            {
                EventType = "Book Check-out",
                EventDescription = x.Book.Title ?? "Unknown Book",
                EventData = x.StatusId == (long)TransectionStatusEnum.Overdue ? nameof(TransectionStatusEnum.Overdue) : "Due soon"
            })
            .ToList();

        var reservationNotifications = await _repositoryManager.ReservationRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.StatusId == (long)ReservationsStatusEnum.Allocated && x.IsAllocated && x.UserId == authUserId)
            .Select(x => new NotificationData
            {
                EventType = "Book Reserved",
                EventDescription = x.Book.Title,
                EventData = "Allocated"
            })
            .ToListAsync();

        var penaltyNotification = await unpaidPenalty
            .Select(x => new NotificationData
            {
                EventType = "Un-paid penalty",
                EventDescription = x.PenaltyType.Label,
                EventData = x.Amount.ToString()
            })
            .ToListAsync();

        if (reservationNotifications.Any())
            notifications.AddRange(reservationNotifications);

        if (penaltyNotification.Any())
            notifications.AddRange(penaltyNotification);

        var checkoutsVisualization = TransectionVisualizationData(action: TransectionActionEnum.Borrow, userId: authUserId);
        var reservationVisualization = ReservationVisualizationData(action: ReservationActionEnum.Reserve, userId: authUserId);
        var genreVisualization = await GenreVisualizationData(userId: authUserId);

        var membershipPopOver = string.Empty;

        if (activeMembershipDetails != null)
        {
            var remainingDays = (activeMembershipDetails.ExpirationDate - DateTimeOffset.UtcNow).Days;
            if (remainingDays <= 2)
            {
                notifications.Add(new NotificationData()
                {
                    EventType = "Your Membership",
                    EventDescription = "Near to Expire",
                    EventData = activeMembershipDetails.ExpirationDate.ToString("g")
                });

                membershipPopOver = $"Your membership expired soon within {remainingDays} Days";
            }
            else
            {
                membershipPopOver = "Your current membership type";
            }
        }
        else
        {
            membershipPopOver = "You need an active membership to check out this book";
        }

        var dashboardData = new UserDashboardDto()
        {
            User = _httpContext!.GetUserName(),
            MembershipLabelPopOverText = membershipPopOver,
            MembreshipLabel = activeMembershipDetails?.MembershipType,
            BorrowLimit = activeMembershipDetails?.BorrowLimit ?? 0,
            UnPaidPenaltyAmount = await unpaidPenalty.SumAsync(x => x.Amount),
            ReservationLimit = activeMembershipDetails?.ReservationLimit ?? 0,
            RecentCheckOuts = await transectionData.Take(5).ProjectTo<RecentCheckOuts>(_mapper.ConfigurationProvider).ToListAsync(),
            GenreVisualization = genreVisualization,
            ReservationVisualization = reservationVisualization,
            CheckoutsVisualization = checkoutsVisualization,
            Notifications = notifications
        };

        return dashboardData;
    }

    public async Task<LibrarianDashboardDto> LibrarianDashboardDataAync()
    {
        var activeMembershipCount = await _repositoryManager.UserMembershipMappingRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .LongCountAsync();

        var transectionData = _repositoryManager.TransectionRepository.GetTransectionOrderByActiveStatus(isActive: true);

        var totalReservations = await _repositoryManager.ReservationRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.StatusId == (long)ReservationsStatusEnum.Reserved)
            .LongCountAsync();

        var unpaidPenalty = _repositoryManager.PenaltyRepository.FindByCondition(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid);

        var checkoutsVisualization = TransectionVisualizationData(action: TransectionActionEnum.Borrow);
        var returnVisualization = TransectionVisualizationData(action: TransectionActionEnum.Return);
        var reservationVisualization = ReservationVisualizationData(action: ReservationActionEnum.Reserve);
        var cancelVisualization = ReservationVisualizationData(action: ReservationActionEnum.Cancel);

        var dashboardData = new LibrarianDashboardDto()
        {
            User = _httpContext!.GetUserName(),
            TotalActiveMembership = activeMembershipCount,
            TotalCheckout = await transectionData.LongCountAsync(),
            TotalReservation = totalReservations,
            UnpaidPenaltyAmount = await unpaidPenalty.SumAsync(x => x.Amount),
            RecentCheckOuts = await transectionData.Where(x => x.StatusId != (long)TransectionStatusEnum.Overdue).Take(5).ProjectTo<RecentCheckOuts>(_mapper.ConfigurationProvider).ToListAsync(),
            OverdueCheckouts = await transectionData.Where(x => x.StatusId == (long)TransectionStatusEnum.Overdue).Take(5).ProjectTo<OverdueCheckOuts>(_mapper.ConfigurationProvider).ToListAsync(),
            UnpaidPenalty = await unpaidPenalty.Take(5).ProjectTo<PenaltyData>(_mapper.ConfigurationProvider).ToListAsync(),
            CheckoutsVisualization = checkoutsVisualization,
            ReturnVisualization = reservationVisualization,
            ReservationVisualization = reservationVisualization,
            CancelVisualization = cancelVisualization
        };

        return dashboardData;
    }

    public async Task<AdminDashboardDto> AdminDashboardDataAync()
    {
        var userMembershipData = await _repositoryManager.UserMembershipMappingRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .LongCountAsync();

        var transectionData = _repositoryManager.TransectionRepository.GetTransectionOrderByActiveStatus(isActive: true);
        var reservationData = _repositoryManager.ReservationRepository.GetAllAsync(isActive: true);
        var unpaidPenaltyAmount = await _repositoryManager.PenaltyRepository.FindByCondition(x => x.IsActive && x.StatusId == (long)FineStatusEnum.UnPaid).SumAsync(x => x.Amount);

        var checkoutsVisualization = TransectionVisualizationData(action: TransectionActionEnum.Borrow);
        var returnVisualization = TransectionVisualizationData(action: TransectionActionEnum.Return);
        var lostVisualization = TransectionVisualizationData(action: TransectionActionEnum.ClaimLost);
        var reservationVisualization = ReservationVisualizationData(action: ReservationActionEnum.Reserve);
        var cancelVisualization = ReservationVisualizationData(action: ReservationActionEnum.Cancel);
        var genreVisualization = await GenreVisualizationData();

        var dashboardData = new AdminDashboardDto()
        {
            User = _httpContext!.GetUserName(),
            TotalActiveMembership = userMembershipData,
            TotalActiveCheckout = await transectionData.Where(x => new[] { (long)TransectionStatusEnum.Borrowed, (long)TransectionStatusEnum.Renewed }.Contains(x.StatusId)).LongCountAsync(),
            TotalOverdueCheckout = await transectionData.Where(x => x.StatusId == (long)TransectionStatusEnum.Overdue).LongCountAsync(),
            TotalLostBooksFromCheckouts = await transectionData.Where(x => x.StatusId == (long)TransectionStatusEnum.ClaimedLost).LongCountAsync(),
            TotalCheckout = await transectionData.LongCountAsync(),
            TotalActiveReservation = await reservationData.Where(x => new[] { (long)ReservationsStatusEnum.Reserved, (long)ReservationsStatusEnum.Allocated }.Contains(x.StatusId)).LongCountAsync(),
            TotalReservation = await reservationData.LongCountAsync(),
            UnPaidPenaltyAmount = unpaidPenaltyAmount,
            RecentCheckOuts = await transectionData.Where(x => x.StatusId != (long)TransectionStatusEnum.Overdue).Take(5).ProjectTo<RecentCheckOuts>(_mapper.ConfigurationProvider).ToListAsync(),
            OverdueCheckouts = await transectionData.Where(x => x.StatusId == (long)TransectionStatusEnum.Overdue).Take(5).ProjectTo<OverdueCheckOuts>(_mapper.ConfigurationProvider).ToListAsync(),
            CheckoutsVisualization = checkoutsVisualization,
            ReturnVisualization = returnVisualization,
            LostVisualization = lostVisualization,
            ReservationVisualization = reservationVisualization,
            CancelVisualization = cancelVisualization,
            GenreVisualization = genreVisualization,
        };

        return dashboardData;
    }

    #region Private section
    private async Task<List<ChartData>> GenreVisualizationData(long? userId = null, int? visualizingMonth = null)
    {
        var genreVisualization = await _repositoryManager.TransectionRepository
            .FindByCondition(x => x.IsActive
                && (userId == null || x.UserId == userId)
                && x.StatusId != (long)TransectionStatusEnum.Cancelled
                && x.CreatedAt.Year == DateTimeOffset.UtcNow.Year
                && x.CreatedAt.Month == (visualizingMonth ?? DateTimeOffset.UtcNow.Month))
            .GroupBy(x => x.Book.Genre.Name)
            .OrderBy(x => x.Key)
            .Select(x => new ChartData
            {
                Label = x.Key,
                Data = x.LongCount()
            })
            .ToListAsync();

        return genreVisualization;
    }

    private List<ChartData> ReservationVisualizationData(ReservationActionEnum action, long? userId = null)
    {
        var currentYear = DateTimeOffset.UtcNow.Year;
        int currentMonth = DateTimeOffset.UtcNow.Month;
        var allMonths = Enumerable.Range(1, currentMonth);

        var reservationData = _repositoryManager.ReservationRepository.FindByCondition(x => x.IsActive && x.CreatedAt.Year == currentYear && (userId == null || x.UserId == userId));

        var reservationVisualization = allMonths.Select(month => new ChartData
        {
            Label = new DateTime(currentYear, month, 1).ToString("MMMM"),
            Data = action switch
            {
                ReservationActionEnum.Reserve => reservationData
                    .Where(x => x.ReservationDate.Month == month)
                    .LongCount(),

                ReservationActionEnum.Cancel => reservationData
                    .Where(x => x.StatusId == (long)ReservationsStatusEnum.Cancelled && x.CancelDate.HasValue && x.CancelDate.Value.Month == month)
                    .LongCount(),

                _ => 0
            }
        }).ToList();

        return reservationVisualization;
    }

    private List<ChartData> TransectionVisualizationData(TransectionActionEnum action, long? userId = null)
    {
        var currentYear = DateTimeOffset.UtcNow.Year;
        int currentMonth = DateTimeOffset.UtcNow.Month;
        var allMonths = Enumerable.Range(1, currentMonth);

        var transectionData = _repositoryManager.TransectionRepository.FindByCondition(x => x.IsActive && x.CreatedAt.Year == currentYear && (userId == null || x.UserId == userId));

        var transectionVisualization = allMonths.Select(month => new ChartData
        {
            Label = new DateTime(currentYear, month, 1).ToString("MMMM"),
            Data = action switch
            {
                TransectionActionEnum.Borrow => transectionData
                    .Where(x => x.BorrowDate.Month == month)
                    .LongCount(),

                TransectionActionEnum.Return => transectionData
                    .Where(x => x.ReturnDate.HasValue && x.ReturnDate.Value.Month == month)
                    .LongCount(),

                TransectionActionEnum.ClaimLost => transectionData
                    .Where(x => x.LostClaimDate.HasValue && x.LostClaimDate.Value.Month == month)
                    .LongCount(),

                _ => 0
            }
        }).ToList();

        return transectionVisualization;
    }
    #endregion
}
