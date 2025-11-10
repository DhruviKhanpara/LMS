using FluentValidation;
using LMS.Application.Contracts.DTOs.Penalty;

namespace LMS.Application.Services.Validations.Penalty;

public class GetPenaltyDtoValidator : AbstractValidator<GetPenaltyDto>
{
    public GetPenaltyDtoValidator()
    {
        RuleFor(penalty => penalty.Id)
            .NotEmpty().WithMessage("Penalty Id is required");

        RuleFor(penalty => penalty.StatusLabel)
            .NotEmpty().WithMessage("Status Label is required")
            .MaximumLength(50).WithMessage("Status Label must be of length 50 characters");

        RuleFor(penalty => penalty.StatusLabelColor)
            .NotEmpty().WithMessage("Status Label is required")
            .MaximumLength(50).WithMessage("Status Label color must be of length 50 characters");

        RuleFor(penalty => penalty.StatusLabelBgColor)
            .NotEmpty().WithMessage("Status Label is required");

        RuleFor(penalty => penalty.TransectionId)
            .Must(x => x == null || x > 0).WithMessage("Transection Id must be null or greater than 0.");

        RuleFor(penalty => penalty.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(50).WithMessage("Penalty Description must be at least 50 characters long.")
            .MaximumLength(500).WithMessage("Penalty Description must be at most 500 characters long.");

        RuleFor(penalty => penalty.Amount)
            .NotNull().WithMessage("Amount can't be null")
            .GreaterThan(0).WithMessage("Penalty must more than 0");

        RuleFor(penalty => penalty.OverDueDays)
            .Must(x => x == null || x > 0).WithMessage("OverDueDays must be null or greater than 0");

        RuleFor(book => book.TransectionDueDate)
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Transection Due date must be in a valid format.")
            .Must(date => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Transection due date cannot be in the future.");

        RuleFor(penalty => penalty.IsRemoved)
            .Must(value => value == true || value == false).WithMessage("IsRemoved flag is required");
    }
}
