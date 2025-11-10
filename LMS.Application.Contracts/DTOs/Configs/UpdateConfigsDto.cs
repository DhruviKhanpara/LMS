using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Configs;

public class UpdateConfigsDto
{
    public long Id { get; set; }
    public string KeyName { get; set; } = null!;
    public string KeyValue { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long? CreatedBy { get; set; }
    public long? ModifiedBy { get; set; }
    public List<SelectListItem> CreateUserList { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> ModifiedUserList { get; set; } = new List<SelectListItem>();
}
