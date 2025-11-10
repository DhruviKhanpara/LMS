using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Penalty;

public class GetPenaltyDto
{
    public long Id { get; set; }
    [Display(Name = "Username")]
    public string UserName { get; set; } = null!;
    public string? UserProfilePhoto { get; set; }
    public long? TransectionId { get; set; }
    [Display(Name = "Transaction Due Date")]
    public DateTimeOffset? TransectionDueDate { get; set; }
    [Display(Name = "Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    [Display(Name = "Penalty Type")]
    public string PenaltyTypeName { get;set; } = null!;
    [Display(Name = "Penalty Type Info")]
    public string PenaltyTypeInfo { get;set; } = null!;
    [Display(Name = "Notes")]
    public string Description { get; set; } = null!;
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
    [Display(Name = "OverDue Days")]
    public int? OverDueDays { get; set; }
    public bool IsRemoved { get; set; } = false;
}
