using FluentValidation;
using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Application.Services.Notification;
using LMS.Application.Services.Notification.Dispatcher;
using LMS.Application.Services.Notification.Handlers;
using LMS.Application.Services.Notification.Processor;
using LMS.Application.Services.Services;
using LMS.Application.Services.Servicesl;
using LMS.Application.Services.Validations.User;
using LMS.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Application.Services.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        services.AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();
        services.AddScoped<IValidationService, ValidationService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IUserMembershipMappingService, UserMembershipMappingService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ITransectionService, TransectionService>();
        services.AddScoped<IPenaltyService, PenaltyService>();
        services.AddScoped<IConfigsService, ConfigsService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<ILogService, LogService>();

        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<TokenService>();

        services.AddScoped<INotificationDispatcher, OutboxEmailDispatcher>();
        services.AddScoped<IOutboxMessageHandler, EmailOutboxMessageHandler>();
        services.AddScoped<IOutboxProcessor, GenericOutboxProcessor>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IJobService, JobService>();

        return services;
    }
}
