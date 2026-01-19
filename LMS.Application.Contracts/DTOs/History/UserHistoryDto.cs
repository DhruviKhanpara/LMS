namespace LMS.Application.Contracts.DTOs.Log;

public class UserHistoryDto : LogAuditDto
{
    public string? Role { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateTimeOffset? Dob { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? MobileNo { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? ProfilePhoto { get; set; }
    public string? LibraryCardNumber { get; set; }
    public DateTimeOffset? JoiningDate { get; set; }
}
