using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Transection;

public class GetUserTransectionDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    [Display(Name = "Book cover page")]
    public string BookCoverPage { get; set; } = null!;
    [Display(Name = "Book")]
    public string BookName { get; set; } = null!;
    [Display(Name = "Author")]
    public string BookAuthor { get; set; } = null!;
    [Display(Name = "Available copies")]
    public long BookAvailableCopies { get; set; }
    [Display(Name = "Total copies")]
    public long BookTotalCopies { get; set; }
    [Display(Name = "Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    [Display(Name = "Borrow date")]
    public DateTimeOffset BorrowDate { get; set; }
    [Display(Name = "Renew count")]
    public int RenewCount { get; set; }
    [Display(Name = "Renew date")]
    public DateTimeOffset? RenewDate { get; set; }
    [Display(Name = "Due date")]
    public DateTimeOffset DueDate { get; set; }
    [Display(Name = "Return date")]
    public DateTimeOffset? ReturnDate { get; set; }
    [Display(Name = "Lost claim date")]
    public DateTimeOffset? LostClaimDate { get; set; }
    public bool HasPenalty { get; set; }
    public bool CanRenewBook { get; set; }
}
