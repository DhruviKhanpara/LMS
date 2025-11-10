using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum TransectionActionEnum : long
{
    [Display(Name = "Cancel", Description = "Cancel book transection operation")]
    Cancel = 1,

    [Display(Name = "Return", Description = "Return the book operation or status")]
    Return = 2,

    [Display(Name = "Renew", Description = "Renew the book operation")]
    Renew = 3,

    [Display(Name = "Delete", Description = "Delete the book operation")]
    Delete = 4,

    [Display(Name = "Claim Lost", Description = "Claim Lost for the book operation or status")]
    ClaimLost = 5,

    [Display(Name = "Borrow", Description = "Claim Lost for the book status")]
    Borrow = 6,
}
