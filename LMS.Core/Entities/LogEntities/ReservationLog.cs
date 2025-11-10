using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class ReservationLog : LogAudit
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
	public DateTimeOffset? ReservationDate { get; set; }
	public DateTimeOffset? AllocateAfter { get; set; }
	public bool? IsAllocated { get; set; }
	public DateTimeOffset? AllocatedAt { get; set; }
	public int? TransferAllocationCount { get; set; }
	public DateTimeOffset? CancelDate { get; set; }
	public string? CancelReason { get; set; }
}
