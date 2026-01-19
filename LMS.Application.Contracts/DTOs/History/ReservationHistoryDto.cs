namespace LMS.Application.Contracts.DTOs.Log;

public class ReservationHistoryDto : LogAuditDto
{
    public string? UserName { get; set; }
    public string? BookName { get; set; }
    public string? StatusLabel { get; set; }
	public string? StatusLabelColor { get; set; }
	public string? StatusLabelBgColor { get; set; }
	public DateTimeOffset? ReservationDate { get; set; }
    public DateTimeOffset? AllocateAfter { get; set; }
    public bool? IsAllocated { get; set; }
    public DateTimeOffset? AllocatedAt { get; set; }
    public int? TransferAllocationCount { get; set; }
    public DateTimeOffset? CancelDate { get; set; }
    public string? CancelReason { get; set; }
}
