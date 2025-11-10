using FluentValidation;
using LMS.Application.Contracts.DTOs.BookFileMapping;

namespace LMS.Application.Services.Validations.BookFileMapping;

public class GetBookFileMappingDtoValidator : AbstractValidator<GetBookFileMappingDto>
{
    public GetBookFileMappingDtoValidator()
    {
        RuleFor(bookFileMapping => bookFileMapping.Id)
            .NotEmpty().WithMessage("Book File Mapping Id is required")
            .GreaterThan(0).WithMessage("Book File Mapping Id ,must be grester than 0");

        RuleFor(bookFileMapping => bookFileMapping.Label)
            .NotEmpty().WithMessage("File label is required");

        RuleFor(bookFileMapping => bookFileMapping.fileLocation)
            .NotEmpty().WithMessage("File location is required");
    }
}
