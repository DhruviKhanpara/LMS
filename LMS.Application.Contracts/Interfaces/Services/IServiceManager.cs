using LMS.Application.Contracts.Interfaces.Notification;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IServiceManager
{
    IUserService UserService { get; }
    IMembershipService MembershipService { get; }
    IUserMembershipMappingService UserMembershipMappingService { get; }
    IBookService BookService { get; }
    IGenreService GenreService { get; }
    IReservationService ReservationService { get; }
    ITransectionService TransectionService{ get; }
    IPenaltyService PenaltyService { get; }
    IConfigsService ConfigsService { get; }
    IHomeService HomeService { get; }
    ILogService LogService { get; }
    INotificationService NotificationService { get; }
}
