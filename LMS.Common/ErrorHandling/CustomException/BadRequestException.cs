namespace LMS.Common.ErrorHandling.CustomException;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
