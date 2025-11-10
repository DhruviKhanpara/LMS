using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities.LogEntities;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace LMS.Infrastructure.Repositories;

internal sealed class RepositoryManager : IRepositoryManager
{
    private readonly LibraryManagementSysContext _context;
    private readonly Lazy<IBookRepository> _booksRepository;
    private readonly Lazy<IConfigRepository> _configRepository;
    private readonly Lazy<IGenreRepository> _genreRepository;
    private readonly Lazy<IMembershipRepository> _membershipRepository;
    private readonly Lazy<IOutboxMessageRepository> _outboxMessageRepository;
    private readonly Lazy<IPenaltyRepository> _penaltyRepository;
    private readonly Lazy<IReservationRepository> _reservationRepository;
    private readonly Lazy<ITransectionRepository> _transectionRepository;
    private readonly Lazy<IUnitOfWork> _unitOfWork;
    private readonly Lazy<IUserMembershipReporsitory> _userMembershipMappingRepository;
    private readonly Lazy<IUserRepository> _userRepository;

    private readonly Lazy<ILogRepositoryBase<BooksLog>> _booksLogRepository;
    private readonly Lazy<ILogRepositoryBase<ConfigsLog>> _configsLogRepository;
    private readonly Lazy<ILogRepositoryBase<GenreLog>> _genreLogRepository;
    private readonly Lazy<ILogRepositoryBase<MembershipLog>> _membershipLogRepository;
    private readonly Lazy<ILogRepositoryBase<PenaltyLog>> _penaltyLogRepository;
    private readonly Lazy<ILogRepositoryBase<ReservationLog>> _reservationLogRepository;
    private readonly Lazy<ILogRepositoryBase<TransectionLog>> _transectionLogRepository;
    private readonly Lazy<ILogRepositoryBase<UserLog>> _userLogRepository;
    private readonly Lazy<ILogRepositoryBase<UserMembershipMappingLog>> _userMembershipMappingLogRepository;

    public RepositoryManager(LibraryManagementSysContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _booksRepository = new Lazy<IBookRepository>(() => new BookRepository(_context));
        _configRepository = new Lazy<IConfigRepository>(() => new ConfigRepository(_context));
        _genreRepository = new Lazy<IGenreRepository>(() => new GenreRepository(_context));
        _membershipRepository = new Lazy<IMembershipRepository>(() => new MembershipRepository(_context));
        _outboxMessageRepository = new Lazy<IOutboxMessageRepository>(() => new OutboxMessageRepository(_context));
        _penaltyRepository = new Lazy<IPenaltyRepository>(() => new PenaltyRepository(_context));
        _reservationRepository = new Lazy<IReservationRepository>(() => new ReservationRepository(_context));
        _transectionRepository = new Lazy<ITransectionRepository>(() => new TransectionRepository(_context));
        _unitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(_context, httpContextAccessor));
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context));
        _userMembershipMappingRepository = new Lazy<IUserMembershipReporsitory>(() => new UserMembershipReporsitory(_context));

        _booksLogRepository = new Lazy<ILogRepositoryBase<BooksLog>>(() => new LogRepositoryBase<BooksLog>(_context));
        _configsLogRepository = new Lazy<ILogRepositoryBase<ConfigsLog>>(() => new LogRepositoryBase<ConfigsLog>(_context));
        _genreLogRepository = new Lazy<ILogRepositoryBase<GenreLog>>(() => new LogRepositoryBase<GenreLog>(_context));
        _membershipLogRepository = new Lazy<ILogRepositoryBase<MembershipLog>>(() => new LogRepositoryBase<MembershipLog>(_context));
        _penaltyLogRepository = new Lazy<ILogRepositoryBase<PenaltyLog>>(() => new LogRepositoryBase<PenaltyLog>(_context));
        _reservationLogRepository = new Lazy<ILogRepositoryBase<ReservationLog>>(() => new LogRepositoryBase<ReservationLog>(_context));
        _transectionLogRepository = new Lazy<ILogRepositoryBase<TransectionLog>>(() => new LogRepositoryBase<TransectionLog>(_context));
        _userLogRepository = new Lazy<ILogRepositoryBase<UserLog>>(() => new LogRepositoryBase<UserLog>(_context));
        _userMembershipMappingLogRepository = new Lazy<ILogRepositoryBase<UserMembershipMappingLog>>(() => new LogRepositoryBase<UserMembershipMappingLog>(_context));
    }

    public IBookRepository BooksRepository => _booksRepository.Value;
    public IConfigRepository ConfigRepository => _configRepository.Value;
    public IGenreRepository GenreRepository => _genreRepository.Value;
    public IMembershipRepository MembershipRepository => _membershipRepository.Value;
    public IOutboxMessageRepository OutboxMessageRepository => _outboxMessageRepository.Value;
    public IPenaltyRepository PenaltyRepository => _penaltyRepository.Value;
    public IReservationRepository ReservationRepository => _reservationRepository.Value;
    public ITransectionRepository TransectionRepository => _transectionRepository.Value;
    public IUnitOfWork UnitOfWork => _unitOfWork.Value;
    public IUserRepository UserRepository => _userRepository.Value;
    public IUserMembershipReporsitory UserMembershipMappingRepository => _userMembershipMappingRepository.Value;

    public ILogRepositoryBase<BooksLog> BooksLogRepository => _booksLogRepository.Value;
    public ILogRepositoryBase<ConfigsLog> ConfigsLogRepository => _configsLogRepository.Value;
    public ILogRepositoryBase<GenreLog> GenreLogRepository => _genreLogRepository.Value;
    public ILogRepositoryBase<MembershipLog> MembershipLogRepository => _membershipLogRepository.Value;
    public ILogRepositoryBase<PenaltyLog> PenaltyLogRepository => _penaltyLogRepository.Value;
    public ILogRepositoryBase<ReservationLog> ReservationLogRepository => _reservationLogRepository.Value;
    public ILogRepositoryBase<TransectionLog> TransectionLogRepository => _transectionLogRepository.Value;
    public ILogRepositoryBase<UserLog> UserLogRepository => _userLogRepository.Value;
    public ILogRepositoryBase<UserMembershipMappingLog> UserMembershipMappingLogRepository => _userMembershipMappingLogRepository.Value;
}
