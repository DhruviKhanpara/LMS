using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class UserMembershipMapping : Audit
{
    #region Table References
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    [ForeignKey(nameof(MembershipId))]
    public virtual Membership Membership { get; set; } = null!;
    #endregion

    public long Id { get; set; }
    public long UserId { get; set; }
    public long MembershipId { get; set; }
    public DateTimeOffset EffectiveStartDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public long BorrowLimit { get; set; }
    public long ReservationLimit { get; set; }
    public decimal MembershipCost { get; set; }
    public decimal Discount { get; set; }
    public decimal? PaidAmount { get; set; }
}
