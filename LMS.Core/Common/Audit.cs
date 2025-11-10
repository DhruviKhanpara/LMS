using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Common;

public partial class Audit
{
    #region Table References
    //public virtual User CreatedByUser { get; set; } = null!;
    //public virtual User? ModifiedByUser { get; set; }
    //public virtual User? DeletedByUser { get; set; }
    #endregion

    public bool IsActive { get; set; } = true;
    [ForeignKey(nameof(CreatedBy))]
    public long CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    [ForeignKey(nameof(ModifiedBy))]
    public long? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    [ForeignKey(nameof(DeletedBy))]
    public long? DeletedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
