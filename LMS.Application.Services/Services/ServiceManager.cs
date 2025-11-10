using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace LMS.Application.Services.Services;

internal sealed class ServiceManager : IServiceManager
{
    private readonly IServiceProvider _provider;
    private readonly ConcurrentDictionary<Type, object> _cache = new();

    public ServiceManager(IServiceProvider provider)
    {
        _provider = provider;
    }

    private T Get<T>() where T : class
    {
        return (T)_cache.GetOrAdd(typeof(T), _ => _provider.GetRequiredService<T>());
    }

    public IUserService UserService => Get<IUserService>();
    public IMembershipService MembershipService => Get<IMembershipService>();
    public IUserMembershipMappingService UserMembershipMappingService => Get<IUserMembershipMappingService>();
    public IBookService BookService => Get<IBookService>();
    public IGenreService GenreService => Get<IGenreService>();
    public IReservationService ReservationService => Get<IReservationService>();
    public ITransectionService TransectionService => Get<ITransectionService>();
    public IPenaltyService PenaltyService => Get<IPenaltyService>();
    public IConfigsService ConfigsService => Get<IConfigsService>();
    public IHomeService HomeService => Get<IHomeService>();
    public ILogService LogService => Get<ILogService>();
    public INotificationService NotificationService => Get<INotificationService>();
}
