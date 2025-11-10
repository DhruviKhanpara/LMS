using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class PenaltyLog : LogAudit
{
    #region Table References
    public virtual User? User { get; set; }
    public virtual Transection? Transection { get; set; }
    public virtual Status? Status { get; set; }
    public virtual PenaltyType? PenaltyType { get; set; }
    #endregion

    public long? Id { get; set; }
    public long? UserId { get; set; }
    public long? TransectionId { get; set; }
    public long? StatusId { get; set; }
    public long? PenaltyTypeId { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public int? OverDueDays { get; set; }
}
