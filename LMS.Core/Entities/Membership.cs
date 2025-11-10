using LMS.Core.Common;

namespace LMS.Core.Entities;

public partial class Membership : Audit
{
    public long Id { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long BorrowLimit { get; set; }
    public long ReservationLimit { get; set; }
    public long Duration { get; set; }
    public decimal Cost { get; set; }
    public decimal Discount { get; set; }
}
