
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// Status auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum BookStatusEnum : long
{
    [Display(Name = "Available", Description = "Book is in the library and available for borrowing")]
    Available = 1,

    [Display(Name = "CheckedOut", Description = "Book is checked out by a user")]
    CheckedOut = 2,

    [Display(Name = "Reserved", Description = "Book is reserved by a user but not yet picked up or issued")]
    Reserved = 3,

    [Display(Name = "LostDamaged", Description = "Book is lost or damaged and cannot be borrowed until fixed or replaced")]
    Lost_Damaged = 4,

    [Display(Name = "OnHold", Description = "Book is temporarily unavailable for any reason")]
    OnHold = 13,

    [Display(Name = "Removed", Description = "Book is removed from library wait to collect all copy from user")]
    Removed = 17
}

