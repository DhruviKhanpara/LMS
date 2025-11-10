using System.ComponentModel;

namespace LMS.Application.Contracts.DTOs.Reservation;

public class ExportReservationDto
{
    public long Id { get; set; }
    [DisplayName("User name")]
    public string UserName { get; set; } = null!;
    [DisplayName("Book name")]
    public string BookName { get; set; } = null!;
    [DisplayName("Status")]
    public string StatusLabel { get; set; } = null!;
    [DisplayName("Reservation date")]
    public DateTimeOffset ReservationDate { get; set; }
    [DisplayName("Allocate after")]
    public DateTimeOffset AllocateAfter { get; set; }
    [DisplayName("Is Allocated?")]
    public bool IsAllocated { get; set; }
    [DisplayName("Allocated at")]
    public DateTimeOffset? AllocatedAt { get; set; }
    [DisplayName("Transfer allocation count")]
    public int TransferAllocationCount { get; set; }
    [DisplayName("Cancel date")]
    public DateTimeOffset? CancelDate { get; set; }
    [DisplayName("Cancel reason")]
    public string? CancelReason { get; set; }
    public bool IsRemoved { get; set; } = false;
}
