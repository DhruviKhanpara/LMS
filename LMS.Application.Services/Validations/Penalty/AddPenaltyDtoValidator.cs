using FluentValidation;
using LMS.Application.Contracts.DTOs.Penalty;

namespace LMS.Application.Services.Validations.Penalty;

public class AddPenaltyDtoValidator : AbstractValidator<AddPenaltyDto>
{
    public AddPenaltyDtoValidator()
    {
        RuleFor(penalty => penalty.UserId)
            .NotEmpty().WithMessage("User is required")
            .GreaterThan(0).WithMessage("User Id must be greater than 0.");

        RuleFor(penalty => penalty.StatusId)
            .NotEmpty().WithMessage("Status Label is required")
            .GreaterThan(0).WithMessage("Status Label Id must be greater than 0.");

        RuleFor(penalty => penalty.PenaltyTypeId)
            .NotEmpty().WithMessage("Penalty type is required")
            .GreaterThan(0).WithMessage("Penalty type Id must be greater than 0.");

        RuleFor(penalty => penalty.TransectionId)
            .Must(x => x == null || x > 0).WithMessage("Transection Id must be null or greater than 0.");

        RuleFor(penalty => penalty.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Penalty Description must be at least 10 characters long.")
            .MaximumLength(500).WithMessage("Penalty Description must be at most 500 characters long.");

        RuleFor(penalty => penalty.Amount)
            .NotNull().WithMessage("Amount can't be null")
            .GreaterThan(0).WithMessage("Penalty must more than 0");

        RuleFor(penalty => penalty.OverDueDays)
            .Must(x => x == null || x > 0).WithMessage("OverDueDays must be null or greater than 0");
    }
}
