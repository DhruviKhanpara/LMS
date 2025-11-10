using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Reservation;

public class BaseReservationDto
{
    public long UserId { get; set; }
    public long BookId { get; set; }
    public long StatusId { get; set; }
    public DateTimeOffset ReservationDate { get; set; }
    public DateTimeOffset? AllocateAfter { get; set; }
    public bool IsAllocated { get; set; }
    public DateTimeOffset? AllocatedAt { get; set; }
    public int TransferAllocationCount { get; set; }
    public DateTimeOffset? CancelDate { get; set; }
    public string? CancelReason { get; set; }

    public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Books { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ReservationStatusList { get; set; } = new List<SelectListItem>();
}
