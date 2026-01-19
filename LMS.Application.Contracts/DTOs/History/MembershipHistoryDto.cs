namespace LMS.Application.Contracts.DTOs.Log;

public class MembershipHistoryDto : LogAuditDto
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    public long? BorrowLimit { get; set; }
    public long? ReservationLimit { get; set; }
    public long? Duration { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Discount { get; set; }
}
