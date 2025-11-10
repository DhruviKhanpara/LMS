using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class Reservation : Audit
{
    #region Table References
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    [ForeignKey(nameof(BookId))]
    public virtual Books Book { get; set; } = null!;
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
    #endregion

    public long Id { get; set; }
    public long UserId { get; set; }
    public long BookId { get; set; }
    public long StatusId { get; set; }
    public DateTimeOffset ReservationDate { get; set; }
    public DateTimeOffset AllocateAfter { get; set; }
    public bool IsAllocated { get; set; }
    public DateTimeOffset? AllocatedAt { get; set; }
    public int TransferAllocationCount { get; set; }
    public DateTimeOffset? CancelDate { get; set; }
    public string? CancelReason { get; set; }
}
