using FluentValidation;
using LMS.Application.Contracts.DTOs.Genre;

namespace LMS.Application.Services.Validations.Genre;

public class GetGenreDtoValidator : AbstractValidator<GetGenreDto>
{
    public GetGenreDtoValidator()
    {
        RuleFor(genre => genre.Id)
            .NotEmpty().WithMessage("Genre Id is required")
            .GreaterThanOrEqualTo(0).WithMessage("Genre Id ,must be grester than or equal 0");

        RuleFor(genre => genre.Name)
            .NotEmpty().WithMessage("Genre Type is required");

        RuleFor(genre => genre.Description)
            .NotEmpty().WithMessage("Genre description is required")
            .MinimumLength(5).WithMessage("Genre length must be at least 5");

        RuleFor(genre => genre.TotalActiveBooks)
            .NotEmpty().WithMessage("Total active book count is required")
            .GreaterThanOrEqualTo(0).WithMessage("Total active book count must be at least 0");

        RuleFor(genre => genre.IsRemoved)
            .Must(value => value == true || value == false).WithMessage("IsRemoved flag is required");
    }
}
