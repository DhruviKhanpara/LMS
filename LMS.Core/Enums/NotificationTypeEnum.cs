using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum NotificationTypeEnum : long
{
    [Display(Name = "NewCheckout", Description = "New Check-out added to user account alert")]
    NewCheckout = 1,

    [Display(Name = "OverdueCheckout", Description = "Check-out overdue to user account alert")]
    OverdueCheckout = 2,

    [Display(Name = "RenewCheckout", Description = "Check-out renew to user account alert")]
    RenewCheckout = 3,

    [Display(Name = "DueDateRemainder", Description = "Due date reminder for Check-out")]
    DueDateRemainder = 4,

    [Display(Name = "ReservationAllocation", Description = "Book allocation to user reservation alert")]
    ReservationAllocation = 5,

    [Display(Name = "NewMembership", Description = "New membership is purchase by user alert")]
    NewMembership = 6,

    [Display(Name = "UpgradeMembership", Description = "Last membership is upgraded by purchased membership alert")]
    UpgradeMembership = 7,

    [Display(Name = "MembershipDue", Description = "membership is near to expiration alert")]
    MembershipDue = 8,
}
