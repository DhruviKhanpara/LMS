using Microsoft.AspNetCore.Http;

namespace LMS.Application.Contracts.DTOs.User;

public class ProfilePhotoUpdateDto
{
    public IFormFile? ProfilePhoto { get; set; }
    public string? ProfilePhotoPath { get; set; }
    public bool IsDeletedProfile { get; set; }
}
