using System.ComponentModel;

namespace LMS.Application.Contracts.DTOs.Reservation;

public class GetReservationDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    [DisplayName("User name")]
    public string UserName { get; set; } = null!;
    public string? UserProfilePhoto { get; set; }
    public long BookId { get; set; }
    [DisplayName("Book name")]
    public string BookName { get; set; } = null!;
    [DisplayName("Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
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
