using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Penalty;

public class AddPenaltyDto : BasePenaltyDto
{
    public List<SelectListItem> Users = new List<SelectListItem>();
    public List<SelectListItem> FineStatusList = new List<SelectListItem>();
    public List<SelectListItem> PenaltyTypeList = new List<SelectListItem>();
}
