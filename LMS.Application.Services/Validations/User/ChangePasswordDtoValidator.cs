using FluentValidation;
using LMS.Application.Contracts.DTOs.User;

namespace LMS.Application.Services.Validations.User;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(user => user.EmailOrUsername)
            .NotEmpty().WithMessage("Email or Username is required");

        RuleFor(p => p.Password)
            .MinimumLength(8).WithMessage("Your password length must be at least 8");

        RuleFor(p => p.NewPassword)
            .NotEmpty().WithMessage("Your new password is required")
            .MinimumLength(8).WithMessage("Your new password length must be at least 8")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,16}$")
            .WithMessage("New Password must be 8-16 characters long, Including at least one uppercase and lowercase letter, one digit, and one special character (!@#$%^&*()-+=).");

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("ConfirmPassword is required")
            .Equal(p => p.NewPassword).WithMessage("New Password and ConfirmPassword not match");
    }
}
