using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class Configs : Audit
{
    #region Table References
    [ForeignKey(nameof(CreatedBy))]
    public virtual User? CreatedByUser { get; set; }
    #endregion

    public long Id { get; set; }
    public string KeyName { get; set; } = null!;
    public string KeyValue { get; set; } = null!;
    public string Description { get; set; } = null!;
    public new long? CreatedBy { get; set; }
}
