using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Enums;

public enum MessageTypeEnum : long
{
    [Display(Name = "Email", Description = "Email notification type")]
    Email = 1
}