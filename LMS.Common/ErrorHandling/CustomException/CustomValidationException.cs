using FluentValidation.Results;

public class CustomValidationException : Exception
{
    public List<ValidationError> Errors { get; }

    public CustomValidationException(IEnumerable<ValidationFailure> failures)
        : base("Validation failed for one or more properties.")
    {
        Errors = failures.Select(err => new ValidationError(err.PropertyName, err.ErrorMessage)).ToList();
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
    }
}

public record ValidationError(string PropertyName, string ErrorMessage);