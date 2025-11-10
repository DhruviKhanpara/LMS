namespace LMS.Common.Models;

public class EmailNotificationResult
{
    public EmailNotificationResult(EmailMessage request, DateTime attemptedSendDate, DateTime? sentDate, string? errorMessage) 
    { 
        EmailNotificationRequest = request;
        AttemptedSendDate = attemptedSendDate;
        SentDate = sentDate;
        ErrorMessage = errorMessage;
    }

    public EmailMessage EmailNotificationRequest { get; private set; }
    public DateTime AttemptedSendDate { get; private set; }
    public DateTime? SentDate { get; private set; }
    public string? ErrorMessage { get; private set; }
}
