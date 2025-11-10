using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Notification;

public interface IOutboxMessageHandler
{
    string MessageType { get; }
    Task HandleAsync(OutboxMessage message);
}
