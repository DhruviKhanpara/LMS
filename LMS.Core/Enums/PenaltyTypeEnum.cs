
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// PenaltyType auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum PenaltyTypeEnum : long
{
    [Display(Name = "LateReturnRenew", Description = "Return or Renew the borrow book after due date")]
    LateReturnRenew = 1,

    [Display(Name = "BookDamage", Description = "Book damage is noticed at return time done by the user")]
    BookDamage = 2,

    [Display(Name = "LostBook", Description = "Book is lost by the user")]
    LostBook = 3,

    [Display(Name = "ExtraHoldings", Description = "Books or materials exceed users membership limit")]
    ExtraHoldings = 4,

    [Display(Name = "BooksHeldUnderExpiredMembership", Description = "Borrow / Renew book while the membership was valid but now expired")]
    BooksHeldUnderExpiredMembership = 5,

    [Display(Name = "Other", Description = "Any other reason then define in this list")]
    Other = 6
}

