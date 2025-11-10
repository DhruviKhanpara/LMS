using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.Helpers;
using LMS.Core.Enums;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LMS.Application.Services.Services;

public class JobService : IJobService
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<JobService> _logger;

    public JobService(IServiceManager serviceManager, ILogger<JobService> logger)
    {
        _serviceManager = serviceManager;
        _logger = logger;
    }

    public void RunPenaltyCalculation()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job: RunPenaltyCalculation");

        try
        {
            _serviceManager.PenaltyService.CalculatePenaltyForHoldingBooks();
            stopwatch.Stop();
            _logger.LogInformation("Completed job: RunPenaltyCalculation in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunPenaltyCalculation job failed");
        }
    }

    public void ReallocateForExpiredAllocations()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job: ReallocateForExpiredAllocations");

        try
        {
            _serviceManager.ReservationService.ReallocateExpiredAllocationToReservationAsync();
            stopwatch.Stop();
            _logger.LogInformation("Completing job: ReallocateForExpiredAllocations in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReallocateForExpiredAllocations job failed");
        }
    }

    public void AllocateReservedBooks()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job: AllocateReservedBooks");

        try
        {
            _serviceManager.ReservationService.AllocateBookToReservation();
            stopwatch.Stop();
            _logger.LogInformation("Completing job: AllocateReservedBooks in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AllocateReservedBooks job failed");
        }
    }

    public void DueDateReminder()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job: DueDateReminder");

        try
        {
            _serviceManager.TransectionService.DispatchEmailForDueDateRemainder();
            stopwatch.Stop();
            _logger.LogInformation("Completing job: DueDateReminder in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DueDateReminder job failed");
        }
    }

    public void MembershipDueReminder()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job: MembershipDueReminder");

        try
        {
            _serviceManager.UserMembershipMappingService.DispatchEmailForMembershipDueRemainder();
            stopwatch.Stop();
            _logger.LogInformation("Completing job: MembershipDueReminder in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MembershipDueReminder job failed");
        }
    }

    public void NotifyReservationAllocation()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job: NotifyReservationAllocation");

        try
        {
            _serviceManager.ReservationService.DispatchEmailForReservationAllocation();
            stopwatch.Stop();
            _logger.LogInformation("Completing job: NotifyReservationAllocation in {Duration}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "NotifyReservationAllocation job failed");
        }
    }

    public async Task<List<JobSchedule>> GetJobSchedule(string? jobName = null)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting job schedule fetching");

        try
        {
            var jobSchedule = await _serviceManager.ConfigsService.GetAllSchedules(jobName: jobName);
            stopwatch.Stop();
            _logger.LogInformation("Completing job schedule fetching in {Duration}ms", stopwatch.ElapsedMilliseconds);
            return jobSchedule ?? new List<JobSchedule>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job schedule fetching failed");
            return new List<JobSchedule>();
        }
    }

    public string GenerateCorn(string frequencyType, int frequencyValue = 1, string? time = null)
    {
        return CornGeneratorForJob.GenerateCron(frequencyType: frequencyType, frequencyValue: frequencyValue, time: time);
    }
}
