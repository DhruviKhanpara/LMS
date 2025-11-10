using LMS.Core.Entities.LogEntities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IRepositoryManager
{
    IBookRepository BooksRepository { get; }
    IConfigRepository ConfigRepository { get; }
    IGenreRepository GenreRepository { get; }
    IMembershipRepository MembershipRepository { get; }
    IOutboxMessageRepository OutboxMessageRepository { get; }
    IPenaltyRepository PenaltyRepository { get; }
    IReservationRepository ReservationRepository { get; }
    ITransectionRepository TransectionRepository { get; }
    IUnitOfWork UnitOfWork { get; }
    IUserRepository UserRepository { get; }
    IUserMembershipReporsitory UserMembershipMappingRepository { get; }

    ILogRepositoryBase<BooksLog> BooksLogRepository { get; }
    ILogRepositoryBase<ConfigsLog> ConfigsLogRepository { get; }
    ILogRepositoryBase<GenreLog> GenreLogRepository { get; }
    ILogRepositoryBase<MembershipLog> MembershipLogRepository { get; }
    ILogRepositoryBase<PenaltyLog> PenaltyLogRepository { get; }
    ILogRepositoryBase<ReservationLog> ReservationLogRepository { get; }
    ILogRepositoryBase<TransectionLog> TransectionLogRepository { get; }
    ILogRepositoryBase<UserLog> UserLogRepository { get; }
    ILogRepositoryBase<UserMembershipMappingLog> UserMembershipMappingLogRepository { get; }
}
