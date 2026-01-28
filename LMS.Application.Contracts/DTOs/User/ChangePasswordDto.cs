namespace LMS.Application.Contracts.DTOs.User;

public class ChangePasswordDto
{
    public long? UserId { get; set; } = null;
    public string EmailOrUsername { get; set; } = null!;
    public string? Password { get; set; }
    public string NewPassword { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
