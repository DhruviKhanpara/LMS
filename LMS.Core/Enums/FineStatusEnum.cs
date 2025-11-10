
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
 
namespace LMS.Core.Enums;

/// <summary>
/// Status auto generated enumeration
/// </summary>
[GeneratedCode("TextTemplatingFileGenerator", "10")]
public enum FineStatusEnum : long
{
    [Display(Name = "Paid", Description = "Penalty was paid")]
    Paid = 10,

    [Display(Name = "UnPaid", Description = "Penalty not paid")]
    UnPaid = 11,

    [Display(Name = "Waived", Description = "penalty is canceled or not applied due to valid reasons")]
    Waived = 19
}

