using FluentValidation;
using LMS.Application.Contracts.DTOs.Transection;
using LMS.Core.Enums;

namespace LMS.Application.Services.Validations.Transection;

public class UpdateTransectionDtoValidator : AbstractValidator<UpdateTransectionDto>
{
    public UpdateTransectionDtoValidator()
    {
        RuleFor(transection => transection.Id)
            .NotEmpty().WithMessage("Transection Id is required")
            .GreaterThan(0).WithMessage("Transection Id must be greater than 0.");

        RuleFor(transection => transection.StatusId)
            .NotEmpty().WithMessage("Status is required")
            .GreaterThan(0).WithMessage("Status must be greater than 0");

        RuleFor(transection => transection.BookId)
            .NotEmpty().WithMessage("Book is required")
            .GreaterThan(0).WithMessage("Book must be greater than 0");

        RuleFor(transection => transection.UserId)
            .NotEmpty().WithMessage("User is required")
            .GreaterThan(0).WithMessage("User must be greater than 0");

        RuleFor(transection => transection.BorrowDate)
            .NotEmpty().WithMessage("Borrow date is required")
            .Must(date => date.Date != default(DateTimeOffset)).WithMessage("Borrow date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Borrow date must be in a valid format.")
            .Must(date => date.Date <= DateTimeOffset.UtcNow).WithMessage("Borrow date cannot be in the future.");

        RuleFor(transection => transection.RenewCount)
            .GreaterThanOrEqualTo(0).WithMessage("Renew count needs to be greater than or equal 0");

        RuleFor(transaction => transaction.RenewDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Renew date is required for Renewed status.")
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Renew date must be in a valid format.")
            .Must((transaction, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Renew date cannot be in the future.")
            .GreaterThanOrEqualTo(transaction => transaction.BorrowDate).WithMessage("Renew date must be greater than or equal to Borrow date.")
            .When(transaction => transaction.StatusId == (long)TransectionStatusEnum.Renewed);

        RuleFor(transaction => transaction.LostClaimDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Lost claim date is required for LostClaim status")
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Lost claim date must be in a valid format.")
            .Must((transaction, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Lost claim date cannot be in the future.")
            .GreaterThanOrEqualTo(transaction => transaction.BorrowDate).WithMessage("Lost claim date must be greater than Borrow date.")
            .When(transaction => transaction.StatusId == (long)TransectionStatusEnum.ClaimedLost);

        RuleFor(transection => transection.DueDate)
            .Must(date => date == null || DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Due date must be in a valid format.")
            .GreaterThanOrEqualTo(x => x.BorrowDate).WithMessage("Due date must be greater than Borrow date")
            .When(transaction => transaction.DueDate.HasValue);

        RuleFor(transaction => transaction.ReturnDate)
            .NotNull()
            .When(transaction =>
                transaction.StatusId == (long)TransectionStatusEnum.Returned ||
                transaction.StatusId == (long)TransectionStatusEnum.Cancelled,
                ApplyConditionTo.CurrentValidator)
            .WithMessage("Return date is required when status is Cancelled or Returned.");

        RuleFor(transaction => transaction.ReturnDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Return date must be in a valid format.")
            .Must((transaction, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Return date cannot be in the future.")
            .GreaterThanOrEqualTo(transaction => transaction.BorrowDate).WithMessage("Return date must be greater than or equal to Borrow date.")
            .GreaterThanOrEqualTo(transaction => transaction.RenewDate.Value)
                .When(transaction => transaction.RenewDate.HasValue)
                .WithMessage("Return date must be greater than or equal to Renew date when Renew date is provided.")
            .When(transaction => transaction.ReturnDate.HasValue);
    }
}
