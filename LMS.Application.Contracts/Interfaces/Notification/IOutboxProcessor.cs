namespace LMS.Application.Contracts.Interfaces.Notification;

public interface IOutboxProcessor
{
    Task ProcessAsync();
}
