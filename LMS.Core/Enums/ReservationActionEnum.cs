using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum ReservationActionEnum : long
{
    [Display(Name = "Cancel", Description = "Cancel reservation operation or status")]
    Cancel = 1,

    [Display(Name = "Transfer", Description = "Transfer Allocation operation")]
    Transfer = 2,

    [Display(Name = "Delete", Description = "Delete Reservation operation")]
    Delete = 3,

    [Display(Name = "Reservation", Description = "Reserve status of Reservation")]
    Reserve =4,
}
