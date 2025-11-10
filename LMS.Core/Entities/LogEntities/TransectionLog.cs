using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class TransectionLog : LogAudit
{
	#region Table References
	public virtual User? User { get; set; }
	public virtual Books? Book { get; set; }
	public virtual Status? Status { get; set; }
	#endregion

	public long? Id { get; set; }
    public long? UserId { get; set; }
    public long? BookId { get; set; }
    public long? StatusId { get; set; }
    public DateTimeOffset? BorrowDate { get; set; }
    public int? RenewCount { get; set; }
    public DateTimeOffset? RenewDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public DateTimeOffset? LostClaimDate { get; set; }
}
