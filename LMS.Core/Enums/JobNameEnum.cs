using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum JobNameEnum : long
{
    [Display(Name = "ProcessGenericOutbox", Description = "define process-generic-outbox job")]
    ProcessGenericOutbox = 1,

    [Display(Name = "RemoveExpiredMemberships", Description = "define remove-expired-memberships job")]
    RemoveExpiredMemberships = 2,

    [Display(Name = "PenaltyCalculation", Description = "define penalty-calculation job")]
    PenaltyCalculation = 3,

    [Display(Name = "ReallocateExpiredReservations", Description = "define reallocate-expired-reservations job")]
    ReallocateExpiredReservations = 1,

    [Display(Name = "AllocateReservedBooks", Description = "define allocate-reserved-books job")]
    AllocateReservedBooks = 1,
}
