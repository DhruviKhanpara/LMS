using LMS.Application.Services.DependencyInjection;
using LMS.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Bootstrapper.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddService(configuration);
        return services;
    }
}
