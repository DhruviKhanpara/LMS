using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.User;

public class RegisterUserDto : BaseUserDto
{
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();
}
