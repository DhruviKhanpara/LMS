using LMS.Application.Contracts.DTOs.Membership;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.UserMembershipMapping;

public class AddUserMembershipDto
{
    public long UserId { get; set; }
    public long MembershipId { get; set; }
    public bool? IsUpgradePlan { get; set; }
    public IEnumerable<SelectListItem> AddPlanOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    public IEnumerable<GetMembershipDto> Memberships { get; set; } = new List<GetMembershipDto>();
    public Dictionary<string, bool> DisableAddForUserCondition { get; set; } = new Dictionary<string, bool>();
}
