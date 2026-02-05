using Hangfire;
using LMS.Application.Contracts.Interfaces.Notification;

namespace LMS.Presentation.Configuration;

public static class HangfireConfiguration
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(configuration.GetConnectionString("LibraryManagementSysConnection"));
        });

        services.AddHangfireServer();

        return services;
    }
}
