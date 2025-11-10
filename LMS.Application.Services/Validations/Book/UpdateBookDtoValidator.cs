using FluentValidation;
using LMS.Application.Contracts.DTOs.Books;
using LMS.Application.Services.Validations.BookFileMapping;

namespace LMS.Application.Services.Validations.Book;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(book => book.Id)
            .NotEmpty().WithMessage("Book Id is required")
            .GreaterThan(0).WithMessage("Book Id ,must be grester than 0");

        RuleFor(book => book.GenreId)
            .NotEmpty().WithMessage("Genre name is required");

        RuleFor(book => book.StatusId)
            .NotEmpty().WithMessage("Status Label is required");

        RuleFor(book => book.Isbn)
            .NotEmpty().WithMessage("ISBN number is required")
            .Matches(@"^(?:\d{9}[\dXx]|\d{3}-\d{1,5}-\d{1,7}-\d{1,7}-[\dXx]|\d{3}-\d{1,5}-\d{1,7}-\d{1,7}-\d{1}|\d{13})$")
                .WithMessage("ISBN number must be a valid 10 or 13-digit number, optionally separated by hyphens.");

        RuleFor(book => book.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must be at most 255 characters long.");

        RuleFor(book => book.BookDescription)
            .NotEmpty().WithMessage("Book Description is required")
            .MinimumLength(50).WithMessage("Book Description must be at least 50 characters long.");

        RuleFor(book => book.AuthorDescription)
            .NotEmpty().WithMessage("Author Description is required")
            .MinimumLength(50).WithMessage("Author Description must be at least 50 characters long.");

        RuleFor(book => book.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(255).WithMessage("Author must be at most 255 characters long.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Author must contain only alphabetic characters and spaces.");

        RuleFor(book => book.Publisher)
            .NotEmpty().WithMessage("Publisher is required")
            .MaximumLength(255).WithMessage("Publisher must be at most 255 characters long.");

        RuleFor(book => book.PublishAt)
            .NotEmpty().WithMessage("Publish date is required")
            .Must(date => date.HasValue && date.Value.Date != default(DateTimeOffset)).WithMessage("Publish date must be a valid date.")
            .Must(date => DateTimeOffset.TryParse(date.ToString(), out _)).WithMessage("Publish date must be in a valid format.")
            .Must(date => date.HasValue && date.Value.Date <= DateTimeOffset.UtcNow).WithMessage("Publish date cannot be in the future.");

        RuleFor(book => book.CoverPage)
            .NotNull().When(x => x.IsDeletedCoverPage).WithMessage("Cover page is required.")
            .Must(file => file == null || file.ContentType == "image/jpeg" || file.ContentType == "image/jpg" || file.ContentType == "image/png")
            .WithMessage("Cover page file must be a JPEG or PNG image.");

        RuleFor(book => book.BookPreview)
            .Must(file => file == null || file.ContentType == "application/pdf")
            .WithMessage("Book preview file must be a PDF document.");

        RuleFor(book => book.TotalCopies)
            .NotEmpty().WithMessage("Total copies is required")
            .GreaterThan(0).WithMessage("Total copies must be greater than 0.");

        RuleFor(book => book.AvailableCopies)
            .LessThanOrEqualTo(p => p.TotalCopies).WithMessage("Available copies can not more than Total copies");

        RuleFor(book => book.Price)
            .NotEmpty().WithMessage("Price is required")
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleForEach(x => x.BookFiles)
            .SetValidator(new GetBookFileMappingDtoValidator());
    }
}
