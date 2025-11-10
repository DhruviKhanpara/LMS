
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// Status auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum TransectionStatusEnum : long
{
    [Display(Name = "Returned", Description = "Book is returned to the library")]
    Returned = 5,

    [Display(Name = "Overdue", Description = "User failed to return the book by the due date")]
    Overdue = 6,

    [Display(Name = "Renewed", Description = "Book has been renewed by the user for an extended period")]
    Renewed = 7,

    [Display(Name = "Cancelled", Description = "Borrowing process was cancelled")]
    Cancelled = 8,

    [Display(Name = "Borrowed", Description = "Book is checked out to the user")]
    Borrowed = 14,

    [Display(Name = "ClaimedLost", Description = "The borrower has reported the book loss")]
    ClaimedLost = 20
}

