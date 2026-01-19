namespace LMS.Application.Contracts.DTOs.Log;

public class UserMembershipMappingHistoryDto : LogAuditDto
{
    public string? UserName { get; set; }
    public string? MembershipType { get; set; }
    public DateTimeOffset? EffectiveStartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public long? BorrowLimit { get; set; }
    public long? ReservationLimit { get; set; }
    public decimal? MembershipCost { get; set; }
    public decimal? Discount { get; set; }
    public decimal? PaidAmount { get; set; }
}
