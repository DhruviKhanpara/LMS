namespace LMS.Common.ErrorHandling.CustomException;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
