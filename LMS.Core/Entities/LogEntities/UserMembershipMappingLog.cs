using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class UserMembershipMappingLog : LogAudit
{
    #region Table References
    public virtual User? User { get; set; }
    public virtual Membership? Membership { get; set; }
    #endregion

    public long? Id { get; set; }
    public long? UserId { get; set; }
    public long? MembershipId { get; set; }
    public DateTimeOffset? EffectiveStartDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public long? BorrowLimit { get; set; }
    public long? ReservationLimit { get; set; }
    public decimal? MembershipCost { get; set; }
    public decimal? Discount { get; set; }
    public decimal? PaidAmount { get; set; }
}
