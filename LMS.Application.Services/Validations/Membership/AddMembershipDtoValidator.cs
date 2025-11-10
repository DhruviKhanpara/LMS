using FluentValidation;
using LMS.Application.Contracts.DTOs.Membership;

namespace LMS.Application.Services.Validations.Membership;

public class AddMembershipDtoValidator : AbstractValidator<AddMembershipDto>
{
    public AddMembershipDtoValidator()
    {
        RuleFor(membership => membership.Type)
            .NotEmpty().WithMessage("membership Type is required");

        RuleFor(membership => membership.Description)
            .NotEmpty().WithMessage("membership description is required")
            .MinimumLength(5).WithMessage("Description length must be at least 5");

        RuleFor(membership => membership.BorrowLimit)
            .GreaterThan(0).WithMessage("Borrow limit can't be 0 or less than 0")
            .LessThanOrEqualTo(10).WithMessage("Borrow limit can not greater than 10");

        RuleFor(membership => membership.ReservationLimit)
            .GreaterThan(0).WithMessage("Reservation limit can't be 0 or less than 0")
            .LessThanOrEqualTo(5).WithMessage("Borrow limit can not greater than 5");

        RuleFor(membership => membership.Duration)
            .GreaterThanOrEqualTo(7).WithMessage("Duration can be for 1 week or long to long 6 month")
            .LessThanOrEqualTo(200).WithMessage("Borrow limit can not greater than 200");

        RuleFor(membership => membership.Cost)
            .NotEmpty().WithMessage("Cost is required")
            .GreaterThan(0).WithMessage("Cost can not less or equal 0");

        RuleFor(membership => membership.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount can not less than 0")
            .LessThanOrEqualTo(p => p.Cost).WithMessage("Discount can not more than Cost");
    }
}
