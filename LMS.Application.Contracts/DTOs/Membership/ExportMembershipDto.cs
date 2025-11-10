using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Membership;

public class ExportMembershipDto
{
    public long Id { get; set; }
    [Display(Name = "Membership type")]
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    [Display(Name = "Borrow limit")]
    public long BorrowLimit { get; set; }
    [Display(Name = "Reservation limit")]
    public long ReservationLimit { get; set; }
    [Display(Name = "Duration in Days")]
    public long Duration { get; set; }
    public decimal Cost { get; set; }
    public decimal Discount { get; set; }
    public bool IsRemoved { get; set; }
}
