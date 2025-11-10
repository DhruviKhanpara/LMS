using LMS.Application.Contracts.DTOs;

namespace LMS.Application.Contracts.Interfaces.Notification;

public interface INotificationDispatcher
{
    Task DispatchEmailAsync(EmailRequestDto request);
    Task DispatchEmailAsync(List<EmailRequestDto> requests);
}
