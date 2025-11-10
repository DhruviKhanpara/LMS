namespace LMS.Application.Contracts.DTOs.User;

public class LoginUserDto
{
    public string? EmailOrUsername { get; set; }
    public string? Password { get; set; }
}
