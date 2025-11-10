namespace LMS.Application.Contracts.DTOs.Membership;

public class BaseMembershipDto
{
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long? BorrowLimit { get; set; }
    public long? ReservationLimit { get; set; }
    public long? Duration { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Discount { get; set; }
}
