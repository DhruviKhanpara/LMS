namespace LMS.Application.Contracts.DTOs.Log;

public class TransectionHistoryDto : LogAuditDto
{
    public string? UserName { get; set; }
    public string? BookName { get; set; }
    public string? StatusLabel { get; set; }
	public string? StatusLabelColor { get; set; }
	public string? StatusLabelBgColor { get; set; }
	public DateTimeOffset? BorrowDate { get; set; }
    public int? RenewCount { get; set; }
    public DateTimeOffset? RenewDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public DateTimeOffset? LostClaimDate { get; set; }
}
