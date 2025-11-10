using FluentValidation;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;

namespace LMS.Application.Services.Validations.UserMembershipMapping;

public class AddUserMembershipDtoValidator : AbstractValidator<AddUserMembershipDto>
{
    public AddUserMembershipDtoValidator()
    {
        RuleFor(userMembership => userMembership.UserId)
            .NotEmpty().WithMessage("User Id is required")
            .GreaterThan(0).WithMessage("User Id ,must be grester than 0");

        RuleFor(userMembership => userMembership.MembershipId)
            .NotEmpty().WithMessage("Membership Id is required")
            .GreaterThan(0).WithMessage("Membership Id ,must be grester than 0");

        RuleFor(userMembership => userMembership.IsUpgradePlan)
            .Must(value => value == true || value == false).WithMessage("Plan option selection is required");
    }
}
