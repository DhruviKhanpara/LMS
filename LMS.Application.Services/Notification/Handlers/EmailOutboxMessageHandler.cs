using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.Interfaces.ExternalServices;
using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Newtonsoft.Json;

namespace LMS.Application.Services.Notification.Handlers;

internal class EmailOutboxMessageHandler : IOutboxMessageHandler
{
    private readonly IEmailSender _emailSender;

    public string MessageType => nameof(MessageTypeEnum.Email);

    public EmailOutboxMessageHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task HandleAsync(OutboxMessage message)
    {
        var request = JsonConvert.DeserializeObject<EmailRequestDto>(message.Payload);
        await _emailSender.SendAsync(request);
    }
}
