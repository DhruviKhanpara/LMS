using FluentValidation;
using LMS.Application.Contracts.DTOs.Reservation;

namespace LMS.Application.Services.Validations.Reservation;

public class GetReservationDtoValidator : AbstractValidator<GetReservationDto>
{
    public GetReservationDtoValidator()
    {
        RuleFor(reservation => reservation.Id)
            .NotEmpty().WithMessage("reservation Id is required");

        RuleFor(reservation => reservation.StatusLabel)
            .NotEmpty().WithMessage("Status Label is required")
            .MaximumLength(50).WithMessage("Status Label must be of length 50 characters");

        RuleFor(reservation => reservation.StatusLabelColor)
            .NotEmpty().WithMessage("Status Label is required")
            .MaximumLength(50).WithMessage("Status Label color must be of length 50 characters");

        RuleFor(reservation => reservation.StatusLabelBgColor)
            .NotEmpty().WithMessage("Status Label is required");

        RuleFor(reservation => reservation.BookName)
            .NotEmpty().WithMessage("Book name is required")
            .MaximumLength(255).WithMessage("Title must be at most 255 characters long.");

        RuleFor(reservation => reservation.ReservationDate)
            .NotEmpty().WithMessage("Reservation date is required")
            .Must(date => date.Date != default(DateTimeOffset)).WithMessage("Reservation date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Reservation date must be in a valid format.")
            .Must(date => date.Date <= DateTimeOffset.UtcNow).WithMessage("Reservation date cannot be in the future.");

        RuleFor(reservation => reservation.AllocateAfter)
            .NotEmpty().WithMessage("Allocate after date is required")
            .Must(date => date.Date != default(DateTimeOffset)).WithMessage("Allocate after date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Allocate after date must be in a valid format.");

        RuleFor(reservation => reservation.IsAllocated)
            .Must(value => value == true || value == false).WithMessage("Is allocated flag is required");

        RuleFor(reservation => reservation.TransferAllocationCount)
            .NotEmpty().WithMessage("Transfer allocation count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Transfer allocation count needs to be greater than or equal 0");

        RuleFor(reservation => reservation.CancelDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Cancel date must be in a valid format.")
            .Must((reservation, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Cancel date cannot be in the future.")
            .GreaterThanOrEqualTo(reservation => reservation.ReservationDate).WithMessage("Cancel date must be greater than or equal to Reservation date.")
            .When(reservation => reservation.CancelDate.HasValue);

        RuleFor(reservation => reservation.CancelReason)
            .MinimumLength(5).WithMessage("Cancel reason must be at least 5 characters long")
            .MaximumLength(255).WithMessage("Cancel reason must be at most 255 characters long.");

        RuleFor(reservation => reservation.IsRemoved)
            .Must(value => value == true || value == false).WithMessage("IsRemoved flag is required");
    }
}
