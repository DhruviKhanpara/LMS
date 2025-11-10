using FluentValidation;
using LMS.Application.Contracts.DTOs.User;

namespace LMS.Application.Services.Validations.User;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(user => user.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Matches("^[a-zA-Z]*$").WithMessage("First name can only contain alphabets.")
            .MaximumLength(100).WithMessage("FirstName must be less than 100 characters");

        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Matches("^[a-zA-Z]*$").WithMessage("Last name can only contain alphabets.")
            .MaximumLength(100).WithMessage("LastName must be less than 100 characters");

        RuleFor(user => user.MiddleName)
            .Matches("^[a-zA-Z]*$").WithMessage("Middle name can only contain alphabets.")
            .MaximumLength(100).WithMessage("Middle name must be less than 100 characters");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(user => user.MobileNo)
            .NotEmpty().WithMessage("Mobile Number is required.")
            .Matches(@"^\d{10}$").WithMessage("Mobile Number must be exactly 10 digits.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(255).WithMessage("Address must not exceed 255 characters.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required.")
            .Must(g => g == "Male" || g == "Female" || g == "NotSaid")
            .WithMessage("Gender must be Male, Female, or preferred not to say.");

        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("Birth date is required.")
            .Must(date => date != default).WithMessage("Birth date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Birth date must be in a valid format.")
            .Must(date => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow.AddYears(-3)).WithMessage("Date of Birth must be in the 3 year past");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Your password is required")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).{8,16}$")
            .WithMessage("Password must be 8-16 characters long, Including at least one uppercase and lowercase letter, one digit, and one special character (!@#$%^&*()-+=).");
        
        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("ConfirmPassword is required")
            .Equal(p => p.Password).WithMessage("Password and ConfirmPassword not match");

        RuleFor(user => user.ProfilePhoto)
            .Must(file => file is null || file.ContentType == "image/jpeg" || file.ContentType == "image/jpg" || file.ContentType == "image/png")
            .WithMessage("Profile photo file must be a JPEG or PNG image.");

        RuleFor(user => user.JoiningDate)
            .NotEmpty().WithMessage("Joining date is required")
            .Must(date => date != default).WithMessage("Joining date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Joining date must be in a valid format.")
            .Must(date => date.Date <= DateTimeOffset.UtcNow).WithMessage("Joining Date cannot be in the future");
    }
}
