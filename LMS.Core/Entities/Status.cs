using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class Status : Audit
{
    #region Table References
    [ForeignKey(nameof(CreatedBy))]
    public virtual User? CreatedByUser { get; set; }
    [ForeignKey(nameof(StatusTypeId))]
    public virtual StatusType? StatusType { get; set; }
    public virtual ICollection<Books> Books { get; set; } = new List<Books>();
    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<Transection> Transections { get; set; } = new List<Transection>();
    #endregion

    public long Id { get; set; }
    public long StatusTypeId { get; set; }
    public string Label { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Color { get; set; } = null!;
    public new long? CreatedBy { get; set; }
}
