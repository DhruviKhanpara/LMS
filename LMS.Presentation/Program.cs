using FluentValidation.AspNetCore;
using LMS.Bootstrapper.DependencyInjection;
using LMS.Presentation.Configuration;
using LMS.Presentation.Middlewares;
using Serilog.Events;
using Serilog;
using Hangfire;
using LMS.Presentation.Filter;
using LMS.Core.Enums;
using LMS.Application.Services.Jobs;

namespace LMS.Presentation;

public class Program
{
    public static void Main(string[] args)
    {
        Serilog.Debugging.SelfLog.Enable(msg =>
        {
            var formatted = $"[{DateTime.UtcNow:u}] {msg}{Environment.NewLine}{new string('-', 80)}{Environment.NewLine}";
            File.AppendAllText("serilog-errors.txt", formatted);
        });

        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console()
                .CreateBootstrapLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.BuilConfiguration();

        //serilog
        builder.BuildLogging();

        // Add services to the container.
        builder.Services.AddMvcWithGlobalFilters();

        //Client side fluent validtion setup
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();

        //Automapper setup
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Add Dependency configurations
        builder.Services.AddAuthenticationConfiguration(builder.Configuration);
        builder.Services.AddProjectDependencies(builder.Configuration);
        builder.Services.AddHangfireServices(builder.Configuration);

        // CORS configuration to allow requests from Angular app
        builder.Services.AddCorsConfiguration();

        var app = builder.Build();

        app.UseNToastNotify();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // Use the developer exception page for detailed error information during development.
            app.UseDeveloperExceptionPage();

            // Use this for check error page in develop env.
            //app.UseStatusCodePagesWithReExecute("/Error/{0}");
        }
        else
        {
            // Re-executes the request with the error code parameter for custom error handling
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowSpecificOrigin");
        app.UseMiddleware<ValidateOriginMiddleware>();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseMiddleware<LoggingMiddleware>();

        app.UseAuthentication();
        app.UseMiddleware<JwtCookieMiddleware>();
        app.UseAuthorization();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthorizationFilter(RoleListEnum.Admin) }
        });

        _ = HangfireJobConfigurator.ConfigureHangfireJobs(app.Services);

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
