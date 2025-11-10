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

    public static async Task ConfigureHangfireJobs(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
        var jobSchedules = await jobService.GetJobSchedule();

        foreach (var job in jobSchedules)
        {
            string corn = jobService.GenerateCorn(frequencyType: job.FrequencyType, frequencyValue: job.Interval ?? 0, time: job.Time);

            RegisterSelectedJob(jobName: job.Name, corn: corn);
        }
    }

    public static async Task RefreshHangfireJobs(IServiceProvider serviceProvider, string jobName)
    {
        var scope = serviceProvider.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
        var jobSchedules = await jobService.GetJobSchedule(jobName: jobName);

        foreach (var job in jobSchedules)
        {
            string corn = jobService.GenerateCorn(frequencyType: job.FrequencyType, frequencyValue: job.Interval ?? 0, time: job.Time);

            RegisterSelectedJob(jobName: job.Name, corn: corn);
        }
    }

    #region Private section

    private static void RegisterSelectedJob(string jobName, string corn)
    {
        switch (jobName)
        {
            case "ProcessGenericOutbox":
                RecurringJob.AddOrUpdate<IOutboxProcessor>(
                    "process-generic-outbox",
                    processor => processor.ProcessAsync(),
                    corn);
                break;

            case "PenaltyCalculation":
                RecurringJob.AddOrUpdate<IJobService>(
                    "penalty-calculation",
                    service => service.RunPenaltyCalculation(),
                    corn);
                break;

            case "ReallocateExpiredReservations":
                RecurringJob.AddOrUpdate<IJobService>(
                    "reallocate-expired-reservations",
                    service => service.ReallocateForExpiredAllocations(),
                    corn);
                break;

            case "AllocateReservedBooks":
                RecurringJob.AddOrUpdate<IJobService>(
                    "allocate-reserved-books",
                    service => service.AllocateReservedBooks(),
                    corn);
                break;

            case "DueDateReminder":
                RecurringJob.AddOrUpdate<IJobService>(
                    "due-date-reminder",
                    service => service.DueDateReminder(),
                    corn);
                break;

            case "MembershipDueReminder":
                RecurringJob.AddOrUpdate<IJobService>(
                    "membership-due-reminder",
                    service => service.MembershipDueReminder(),
                    corn);
                break;

            case "NotifyReservationAllocation":
                RecurringJob.AddOrUpdate<IJobService>(
                    "notify-reservation-allocation",
                    service => service.NotifyReservationAllocation(),
                    corn);
                break;
        }
    }
    
    #endregion
}
