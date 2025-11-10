using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class MembershipLog : LogAudit
{
    public long? Id { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public long? BorrowLimit { get; set; }
    public long? ReservationLimit { get; set; }
    public long? Duration { get; set; }
    public decimal? Cost { get; set; }
    public decimal? Discount { get; set; }
}
