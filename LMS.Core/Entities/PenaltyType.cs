using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class PenaltyType : Audit
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public string Description { get; set; } = null!;

    [ForeignKey(nameof(CreatedBy))]
    public new long? CreatedBy { get; set; }
}
