using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class Penalty : Audit
{
    #region Table References
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    [ForeignKey(nameof(TransectionId))]
    public virtual Transection? Transection { get; set; }
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
    [ForeignKey(nameof(PenaltyTypeId))]
    public virtual PenaltyType PenaltyType { get; set; } = null!;
    #endregion

    public long Id { get; set; }
    public long UserId { get; set; }
    public long? TransectionId { get; set; }
    public long StatusId { get; set; }
    public long PenaltyTypeId { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public int? OverDueDays { get; set; }
}
