using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class Transection : Audit
{
    #region Table References
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    [ForeignKey(nameof(BookId))]
    public virtual Books Book { get; set; } = null!;
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
    public virtual List<Penalty> Penalties { get; set; } = new List<Penalty>();
    #endregion

    public long Id { get; set; }
    public long UserId { get; set; }
    public long BookId { get; set; }
    public long StatusId { get; set; }
    public DateTimeOffset BorrowDate { get; set; }
    public int RenewCount { get; set; }
    public DateTimeOffset? RenewDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public DateTimeOffset? LostClaimDate { get; set; }
}
