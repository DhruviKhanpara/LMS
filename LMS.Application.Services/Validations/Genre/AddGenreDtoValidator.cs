using FluentValidation;
using LMS.Application.Contracts.DTOs.Genre;

namespace LMS.Application.Services.Validations.Genre;

public class AddGenreDtoValidator : AbstractValidator<AddGenreDto>
{
    public AddGenreDtoValidator()
    {
        RuleFor(genre => genre.Name)
            .NotEmpty().WithMessage("Genre Type is required");

        RuleFor(genre => genre.Description)
            .NotEmpty().WithMessage("Genre description is required")
            .MinimumLength(5).WithMessage("Genre length must be at least 5");
    }
}
