using FluentValidation;
using LMS.Application.Contracts.DTOs.Genre;

namespace LMS.Application.Services.Validations.Genre;

public class UpdateGenreDtoValidator : AbstractValidator<UpdateGenreDto>
{
    public UpdateGenreDtoValidator()
    {
        RuleFor(genre => genre.Id)
            .NotEmpty().WithMessage("Genre Id is required")
            .GreaterThan(0).WithMessage("Genre Id must be greater than 0.");

        RuleFor(genre => genre.Name)
            .NotEmpty().WithMessage("Genre Type is required");

        RuleFor(genre => genre.Description)
            .NotEmpty().WithMessage("Genre description is required")
            .MinimumLength(5).WithMessage("Genre length must be at least 5");
    }
}
