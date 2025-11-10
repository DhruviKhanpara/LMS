using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum UserMembershipStatusEnum : long
{
    [Display(Name = "Active", Description = "User’s current membership is running")]
    Active = 1,

    [Display(Name = "Upcoming", Description = "User has selected a next membership plan")]
    Upcoming = 2,

    [Display(Name = "Expired", Description = "Membership has ended")]
    Expired = 3,

    [Display(Name = "UnCategorized", Description = "Not in any of above")]
    UnCategorized = 4,
}
    