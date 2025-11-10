using FluentValidation;
using LMS.Application.Contracts.DTOs.Transection;
using LMS.Core.Enums;

namespace LMS.Application.Services.Validations.Transection;

public class GetTransectionDtoValidator : AbstractValidator<GetTransectionDto>
{
    public GetTransectionDtoValidator()
    {
        RuleFor(transection => transection.Id)
            .NotEmpty().WithMessage("Transection Id is required")
            .GreaterThan(0).WithMessage("Transection Id needs to be greater than 0");

        RuleFor(transection => transection.StatusLabel)
            .NotEmpty().WithMessage("Status Label is required")
            .MaximumLength(50).WithMessage("Status Label must be of length 50 characters");

        RuleFor(transection => transection.StatusLabelColor)
            .NotEmpty().WithMessage("Status Label is required")
            .MaximumLength(50).WithMessage("Status Label color must be of length 50 characters");

        RuleFor(transection => transection.StatusLabelBgColor)
            .NotEmpty().WithMessage("Status Label is required");

        RuleFor(transection => transection.BookName)
            .NotEmpty().WithMessage("Book name is required")
            .MaximumLength(255).WithMessage("Title must be at most 255 characters long.");

        RuleFor(transection => transection.BorrowDate)
            .NotEmpty().WithMessage("Borrow date is required")
            .Must(date => date.Date != default(DateTimeOffset)).WithMessage("Borrow date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Borrow date must be in a valid format.")
            .Must(date => date.Date <= DateTimeOffset.UtcNow).WithMessage("Borrow date cannot be in the future.");

        RuleFor(transection => transection.RenewCount)
            .NotEmpty().WithMessage("Renew count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Renew count needs to be greater than or equal 0");

        RuleFor(transaction => transaction.RenewDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Renew date is required for Renewed status.")
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Renew date must be in a valid format.")
            .Must((transaction, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Renew date cannot be in the future.")
            .GreaterThanOrEqualTo(transaction => transaction.BorrowDate).WithMessage("Renew date must be greater than or equal to Borrow date.")
            .When(transaction => transaction.StatusLabel == nameof(TransectionStatusEnum.Renewed));

        RuleFor(transaction => transaction.LostClaimDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Lost claim date is required for LostClaim status")
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Lost claim date must be in a valid format.")
            .Must((transaction, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Lost claim date cannot be in the future.")
            .GreaterThanOrEqualTo(transaction => transaction.BorrowDate).WithMessage("Lost claim date must be greater than Borrow date.")
            .When(transaction => transaction.StatusLabel == nameof(TransectionStatusEnum.ClaimedLost));

        RuleFor(transection => transection.DueDate)
            .Must(date => date.Date != default(DateTimeOffset)).WithMessage("Due date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Due date must be in a valid format.")
            .GreaterThanOrEqualTo(x => x.BorrowDate).WithMessage("Due date must be greater than Borrow date");

        RuleFor(transaction => transaction.ReturnDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Return date must be in a valid format.")
            .Must((transaction, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Return date cannot be in the future.")
            .GreaterThanOrEqualTo(transaction => transaction.BorrowDate).WithMessage("Return date must be greater than or equal to Borrow date.")
            .When(transaction => transaction.ReturnDate.HasValue);

        RuleFor(transection => transection.IsRemoved)
            .Must(value => value == true || value == false).WithMessage("IsRemoved flag is required");
    }
}
