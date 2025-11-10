using Microsoft.AspNetCore.Http;

namespace LMS.Application.Contracts.DTOs.User;

public class BaseUserDto
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset? Dob { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? MobileNo { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public long RoleId { get; set; }
    public DateTimeOffset JoiningDate { get; set; }
    public IFormFile? ProfilePhoto { get; set; }
}
