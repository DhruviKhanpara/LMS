using FluentValidation;
using LMS.Application.Contracts.DTOs.Reservation;
using LMS.Core.Enums;

namespace LMS.Application.Services.Validations.Reservation;

public class UpdateReservationDtoValidator : AbstractValidator<UpdateReservationDto>
{
    public UpdateReservationDtoValidator()
    {
        RuleFor(reservation => reservation.Id)
            .NotEmpty().WithMessage("Reservation Id is required")
            .GreaterThan(0).WithMessage("Reservation Id must be greater than 0.");

        RuleFor(reservation => reservation.StatusId)
            .NotEmpty().WithMessage("Status is required")
            .GreaterThan(0).WithMessage("Status Id must be greater than 0");

        RuleFor(reservation => reservation.BookId)
            .NotEmpty().WithMessage("Book is required")
            .GreaterThan(0).WithMessage("Book Id must be greater than 0");

        RuleFor(reservation => reservation.UserId)
            .NotEmpty().WithMessage("User is required")
            .GreaterThan(0).WithMessage("User Id must be greater than 0");

        RuleFor(reservation => reservation.ReservationDate)
            .NotEmpty().WithMessage("Reservation date is required")
            .Must(date => date.Date != default(DateTimeOffset)).WithMessage("Reservation date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Reservation date must be in a valid format.")
            .Must(date => date.Date <= DateTimeOffset.UtcNow).WithMessage("Reservation date cannot be in the future.");

        RuleFor(reservation => reservation.AllocateAfter)
            .NotEmpty().WithMessage("Allocate after date is required")
            .Must(date => date == null || DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Allocate after date must be in a valid format.")
            .GreaterThanOrEqualTo(x => x.ReservationDate).WithMessage("Allocate after date must be greater than Reservation date")
            .When(reservation => reservation.AllocateAfter.HasValue);

        RuleFor(reservation => reservation.AllocatedAt)
            .Must(date => date == null || DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Allocate date must be in a valid format.")
            .GreaterThanOrEqualTo(x => x.ReservationDate).WithMessage("Allocate date must be greater than Reservation date")
            .When(reservation => reservation.AllocatedAt.HasValue && reservation.AllocateAfter.HasValue && reservation.IsAllocated && reservation.StatusId == (long)ReservationsStatusEnum.Allocated)
            .GreaterThanOrEqualTo(x => x.AllocateAfter)
            .WithMessage("Allocate date must be greater than Allocate After date.");

        RuleFor(reservation => reservation.CancelReason)
            .MinimumLength(5).WithMessage("Cancle reason must be at lease 5 characters long.")
            .MaximumLength(255).WithMessage("Cancle reason must be at most 255 characters long.");

        RuleFor(reservation => reservation.IsAllocated)
            .Must(value => value == true || value == false).WithMessage("IsAllocate flag is required");

        RuleFor(reservation => reservation.TransferAllocationCount)
            .GreaterThanOrEqualTo(0).WithMessage("Transfer allocation count needs to be greater than or equal 0");

        RuleFor(transaction => transaction.CancelDate)
            .NotNull()
            .When(reservation =>
                reservation.StatusId == (long)ReservationsStatusEnum.Cancelled,
                ApplyConditionTo.CurrentValidator)
            .WithMessage("Cancel date is required when status is Cancelled.");

        RuleFor(reservation => reservation.CancelDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => DateTimeOffset.TryParse(date?.ToString(), out _)).WithMessage("Cancel date must be in a valid format.")
            .Must((reservation, date) => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Cancel date cannot be in the future.")
            .GreaterThanOrEqualTo(reservation => reservation.ReservationDate).WithMessage("Cancel date must be greater than or equal to Reservation date.")
            .GreaterThanOrEqualTo(reservation => reservation.AllocatedAt.Value)
                .When(reservation => reservation.AllocatedAt.HasValue)
                .WithMessage("Cancel date must be less than or equal to allocate date when allocation is done once.")
            .When(reservation => reservation.CancelDate.HasValue);
    }
}
