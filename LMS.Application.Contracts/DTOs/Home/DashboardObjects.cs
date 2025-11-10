using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Home;

public class RecentCheckOuts
{
    [Display(Name = "User")]
    public string UserName { get; set; } = null!;
    public string? UserProfilePhoto { get; set; }
    [Display(Name = "Book")]
    public string BookName { get; set; } = null!;
    [Display(Name = "Author")]
    public string BookAuthor { get; set; } = null!;
    [Display(Name = "Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    [Display(Name = "Borrow date")]
    public DateTimeOffset BorrowDate { get; set; }
    [Display(Name = "Renew date")]
    public DateTimeOffset? RenewDate { get; set; }
    [Display(Name = "Due date")]
    public DateTimeOffset DueDate { get; set; }
    [Display(Name = "Return date")]
    public DateTimeOffset? ReturnDate { get; set; }
    [Display(Name = "Lost claim date")]
    public DateTimeOffset? LostClaimDate { get; set; }
}

public class OverdueCheckOuts
{
    [Display(Name = "User")]
    public string UserName { get; set; } = null!;
    public string? UserProfilePhoto { get; set; }
    [Display(Name = "Book")]
    public string BookName { get; set; } = null!;
    [Display(Name = "Overdue Days")]
    public int? OverdueDays { get; set; }
    [Display(Name = "Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
}
public class PenaltyData
{
    [Display(Name = "User")]
    public string UserName { get; set; } = null!;
    public string? UserProfilePhoto { get; set; }
    [Display(Name = "Penalty Type")]
    public string PenaltyTypeName { get; set; } = null!; 
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
}


public class NotificationData
{
    public string EventType { get; set; } = null!;
    public string EventDescription { get; set; } = null!;
    public string? EventData { get; set; }
}

public class ChartData
{
    public string Label { get; set; } = null!;
    public decimal Data { get; set; }
}