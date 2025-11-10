using LMS.Core.Entities;

namespace LMS.Core.Common;

public class LogAudit
{
    #region Table References
    public virtual User? CreatedByUser { get; set; }
    public virtual User? ModifiedByUser { get; set; }
    public virtual User? DeletedByUser { get; set; }
    #endregion

    public long SerialNumber { get; set; }
    public string? Operation { get; set; }
    public string? LogType { get; set; }
    public DateTimeOffset? LogTime { get; set; }
    public bool? IsActive { get; set; } = true;
    public long? CreatedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public long? DeletedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
