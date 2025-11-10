using LMS.Application.Contracts.DTOs.Membership;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.UserMembershipMapping;

public class UpdateUserMembershipDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long MembershipId { get; set; }
    public DateTimeOffset EffectiveStartDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public long BorrowLimit { get; set; }
    public long ReservationLimit { get; set; }
    public decimal MembershipCost { get; set; }
    public decimal Discount { get; set; }
    public decimal? PaidAmount { get; set; }
    public string? UserPassword { get; set; }
    public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    public IEnumerable<GetMembershipDto> Memberships { get; set; } = new List<GetMembershipDto>();
}
