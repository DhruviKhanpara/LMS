using FluentValidation;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;

namespace LMS.Application.Services.Validations.UserMembershipMapping;

public class GetUserMembershipListDtoValidator : AbstractValidator<GetUserMembersipListDto>
{
    public GetUserMembershipListDtoValidator()
    {
        RuleFor(userMembership => userMembership.Id)
            .NotEmpty().WithMessage("membership Id is required")
            .GreaterThanOrEqualTo(0).WithMessage("userMembership Id ,must be grester than or equal 0");

        RuleFor(userMembership => userMembership.MembershipType)
            .NotEmpty().WithMessage("Membership Type is required");

        RuleFor(userMembership => userMembership.MembershipDescription)
            .NotEmpty().WithMessage("Membership description is required");

        RuleFor(userMembership => userMembership.BorrowLimit)
            .GreaterThan(0).WithMessage("Borrow limit can not be 0")
            .LessThanOrEqualTo(10).WithMessage("Borrow limit can not greater than 10");

        RuleFor(userMembership => userMembership.ReservationLimit)
            .GreaterThan(0).WithMessage("Reservation limit can not be 0")
            .GreaterThan(0).WithMessage("Reservation limit can't be 0 or less than 0")
            .LessThanOrEqualTo(5).WithMessage("Borrow limit can not greater than 5");

        RuleFor(userMembership => userMembership.EffectiveStartDate)
            .Must(date => date != default).WithMessage("Effective start date must be a valid date.")
            .NotEmpty().WithMessage("Effective start date is required.");

        RuleFor(userMembership => userMembership.ExpirationDate)
            .NotEmpty().WithMessage("Expiration date is required.")
            .Must(date => date != default).WithMessage("Expiration date must be a valid date.")
            .GreaterThanOrEqualTo(p => p.EffectiveStartDate.AddDays(7)).WithMessage("Expiration date must be greater than start date");

        RuleFor(userMembership => userMembership.MembershipCost)
            .GreaterThan(0).WithMessage("Cost can not less or equal 0");

        RuleFor(userMembership => userMembership.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount can not less than 0")
            .LessThanOrEqualTo(p => p.MembershipCost).WithMessage("Discount can not more than Cost");

        RuleFor(userMembership => userMembership.PaidAmount)
            .Equal(p => p.MembershipCost - p.Discount).WithMessage("Paid amount must be according to the membership cost");

        RuleFor(userMembership => userMembership.IsRemoved)
            .Must(value => value == true || value == false).WithMessage("IsRemoved flag is required");

        RuleFor(userMembership => userMembership.StatusLabel)
            .NotEmpty().WithMessage("Status Label is required");

        RuleFor(userMembership => userMembership.StatusLabelColor)
            .NotEmpty().WithMessage("Status Label is required");

        RuleFor(userMembership => userMembership.StatusLabelBgColor)
            .NotEmpty().WithMessage("Status Label is required");
    }
}
