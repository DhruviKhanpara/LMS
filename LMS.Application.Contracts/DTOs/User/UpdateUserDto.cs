using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.User;

public class UpdateUserDto : BaseUserDto
{
    public long Id { get; set; }
    public string? LibraryCardNumber { get; set; }
    public string? ProfilePhotoPath { get; set; }
    public bool IsDeletedProfile { get; set; }
    public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();
}
