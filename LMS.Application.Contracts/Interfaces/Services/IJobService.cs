using LMS.Application.Contracts.DTOs;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IJobService
{
    void RunPenaltyCalculation();
    void ReallocateForExpiredAllocations();
    void AllocateReservedBooks();
    void DueDateReminder();
    void MembershipDueReminder();
    void NotifyReservationAllocation();
    Task<List<JobSchedule>> GetJobSchedule(string? jobName = null);
    string GenerateCorn(string frequencyType, int frequencyValue = 1, string? time = null);
}
