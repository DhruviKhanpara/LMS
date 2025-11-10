using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Reservation;

public class GetUserReservationDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long BookId { get; set; }
    [Display(Name = "Book cover page")]
    public string BookCoverPage { get; set; } = null!;
    [DisplayName("Book")]
    public string BookName { get; set; } = null!;
    [Display(Name = "Author")]
    public string BookAuthor { get; set; } = null!;
    [Display(Name = "Available copies")]
    public long BookAvailableCopies { get; set; }
    [Display(Name = "Total copies")]
    public long BookTotalCopies { get; set; }
    [DisplayName("Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    [DisplayName("Reservation Date")]
    public DateTimeOffset ReservationDate { get; set; }
    [DisplayName("Allocate after")]
    public DateTimeOffset AllocateAfter { get; set; }
    [DisplayName("Is Allocated?")]
    public bool IsAllocated { get; set; }
    [DisplayName("Allocate Date")]
    public DateTimeOffset? AllocatedAt { get; set; }
    [DisplayName("Transfer allocation count")]
    public int TransferAllocationCount { get; set; }
    [DisplayName("Cancel date")]
    public DateTimeOffset? CancelDate { get; set; }
    [DisplayName("Cancel reason")]
    public string? CancelReason { get; set; }
    public bool CanTransferAllocation { get; set; }
}
