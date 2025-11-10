using FluentValidation;
using LMS.Application.Contracts.DTOs.User;

namespace LMS.Application.Services.Validations.User;

public class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
{
    public UserProfileDtoValidator()
    {

        RuleFor(user => user.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Matches("^[a-zA-Z]*$").WithMessage("First name can only contain alphabets.")
            .MaximumLength(100).WithMessage("FirstName must be of length 100 characters");

        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Matches("^[a-zA-Z]*$").WithMessage("Last name can only contain alphabets.")
            .MaximumLength(100).WithMessage("LastName must be of length 100 characters");

        RuleFor(user => user.MiddleName)
            .Matches("^[a-zA-Z]*$").WithMessage("Middle name can only contain alphabets.")
            .MaximumLength(100).WithMessage("Middle name must be of length 100 characters");

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
            .Must(date => date.Date <= DateTimeOffset.UtcNow.AddYears(-3)).WithMessage("Date of Birth must be in the 3 year past");

        RuleFor(user => user.Id)
            .NotEmpty().WithMessage("User Id is required")
            .GreaterThan(0).WithMessage("User Id must be grester than 0");

        RuleFor(user => user.LibraryCardNumber)
            .NotEmpty().WithMessage("Library Card Number is required")
            .MaximumLength(50).WithMessage("Library Card Number must not exceed 50 characters");

        RuleFor(user => user.JoiningDate)
            .NotEmpty().WithMessage("Joining date is required")
            .Must(date => date != default).WithMessage("Joining date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Joining date must be in a valid format.")
            .Must(date => date.Date <= DateTimeOffset.UtcNow).WithMessage("Joining Date cannot be in the future");

        RuleFor(user => user.PenaltyCount)
            .NotEmpty().WithMessage("Penalty Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Penalty Count must be grester than or equal 0");

        RuleFor(user => user.TotalPenaltyCount)
            .NotEmpty().WithMessage("Total Penalty Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Total Penalty Count must be grester than or equal 0");

        RuleFor(user => user.OccupiedBookCount)
            .NotEmpty().WithMessage("Occupied Book Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Occupied Book Count must be grester than or equal 0");

        RuleFor(user => user.TotalOccupiedBookCount)
            .NotEmpty().WithMessage("Total Occupied Book Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Total Occupied Book Count must be grester than or equal 0");

        RuleFor(user => user.ActiveReservationCount)
            .NotEmpty().WithMessage("Active Reservation Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Active Reservation Count must be grester than or equal 0");

        RuleFor(user => user.TotalReservationCount)
            .NotEmpty().WithMessage("Total Active Reservation Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Total Active Reservation Count must be grester than or equal 0");

        RuleFor(user => user.ActiveMembershipCount)
            .NotEmpty().WithMessage("Active Membership Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Active Membership Count must be grester than or equal 0");

        RuleFor(user => user.TotalMembershipCount)
            .NotEmpty().WithMessage("Total Active Membership Count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Total Active Membership Count must be grester than or equal 0");
    }
}
