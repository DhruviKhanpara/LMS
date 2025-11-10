using LMS.Core.Enums;

namespace LMS.Application.Contracts.Interfaces.Notification;

public interface INotificationService
{
    Task DispatchEmail(NotificationTypeEnum type, object? data);
    Task DispatchMultipleEmail(NotificationTypeEnum type, List<object> dataList);
}
