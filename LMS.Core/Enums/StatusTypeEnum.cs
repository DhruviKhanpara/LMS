
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// StatusType auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum StatusTypeEnum : long
{
    [Display(Name = "Book", Description = "Status related to book availability condition or categorization")]
    Book = 1,

    [Display(Name = "Transaction", Description = "Status related to borrowing returning or renewing books")]
    Transaction = 2,

    [Display(Name = "Fine", Description = "Status related to overdue fines payments and penalties")]
    Fine = 3,

    [Display(Name = "Reservations", Description = "Status related to reserving the book")]
    Reservations = 4
}

