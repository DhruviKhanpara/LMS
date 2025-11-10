using FluentValidation;
using LMS.Application.Contracts.DTOs.User;

namespace LMS.Application.Services.Validations.User;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(user => user.EmailOrUsername)
            .NotEmpty().WithMessage("Email or Username is required");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8");
    }
}
