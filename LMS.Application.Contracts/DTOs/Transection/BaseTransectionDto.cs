using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Transection;

public class BaseTransectionDto
{
    public long UserId { get; set; }
    public long BookId { get; set; }
    public long StatusId { get; set; }
    public DateTimeOffset BorrowDate { get; set; }
    public int RenewCount { get; set; }
    public DateTimeOffset? RenewDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? LostClaimDate { get; set; }
    public bool IsDefaultDueDate { get; set; }
    public bool IsCountRenew { get; set; }

    public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Books { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TransectionStatusList { get; set; } = new List<SelectListItem>();
}
