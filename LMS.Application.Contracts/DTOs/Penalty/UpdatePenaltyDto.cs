using LMS.Application.Contracts.DTOs.Transection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Penalty;

public class UpdatePenaltyDto : BasePenaltyDto
{
    public long Id { get; set; }
    public long? TransectionStatusId { get; set; }
    public List<SelectListItem> Users = new List<SelectListItem>();
    public List<SelectListItem> FineStatusList = new List<SelectListItem>();
    public List<SelectListItem> PenaltyTypeList = new List<SelectListItem>();
    public List<GetTransectionDto> Transections = new List<GetTransectionDto>();
    public List<SelectListItem> TransectionStatus = new List<SelectListItem>();
}
