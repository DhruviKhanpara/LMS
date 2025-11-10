
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// Status auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum ReservationsStatusEnum : long
{
    [Display(Name = "Fulfilled", Description = "The reservation is complete and the book has been borrowed by the user")]
    Fulfilled = 9,

    [Display(Name = "Reserved", Description = "User has reserved the book but hasnt picked it up yet")]
    Reserved = 15,

    [Display(Name = "Cancelled", Description = "User has cancelled the reservation")]
    Cancelled = 16,

    [Display(Name = "Allocated", Description = "Book has been allocated")]
    Allocated = 18
}

