using LMS.Application.Contracts.Interfaces.ExternalServices;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace LMS.Infrastructure.ExternalServices;

internal sealed class ExternalServiceManager : IExternalServiceManager
{
    private readonly IServiceProvider _provider;
    private readonly ConcurrentDictionary<Type, object> _cache = new();

    public ExternalServiceManager(IServiceProvider provider)
    {
        _provider = provider;
    }

    private T Get<T>() where T : class
    {
        return (T)_cache.GetOrAdd(typeof(T), _ => _provider.GetRequiredService<T>());
    }

    public IPdfGenerator PdfGenerator => Get<IPdfGenerator>();
    public IBarcodeGenerator BarcodeGenerator => Get<IBarcodeGenerator>();
    public IEmailSender EmailSender => Get<IEmailSender>();
}
