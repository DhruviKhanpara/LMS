using LMS.Application.Contracts.DTOs.Membership;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;

namespace LMS.Presentation.Views.Shared.Models;

public class PlanSelectionModelBodyViewModel
{
    public string AvailablePlansTitle { get; set; } = "Available Plans";
    public string SelectedPlansTitle { get; set; } = "Selected Plans";
    public long MaxAvailablePlanLimit { get; set; }
    public List<GetUserMembershipDto> AvailablePlans { get; set; } = new List<GetUserMembershipDto>();
    public GetMembershipDto? SelectedPlan { get; set; } = null;
}
