namespace LMS.Application.Contracts.DTOs.Log;

public class PenaltyHistoryDto : LogAuditDto
{
    public string? UserName { get; set; }
    public DateTimeOffset? TransectionDueDate { get; set; }
    public string? StatusLabel { get; set; }
    public string? StatusLabelColor { get; set; }
    public string? StatusLabelBgColor { get; set; }
    public string? PenaltyTypeName { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public int? OverDueDays { get; set; }
}
