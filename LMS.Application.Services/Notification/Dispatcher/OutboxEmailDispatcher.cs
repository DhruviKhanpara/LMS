using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Common.Helpers;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LMS.Application.Services.Notification.Dispatcher;

public class OutboxEmailDispatcher : INotificationDispatcher
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly HttpContext? _httpContext;

    public OutboxEmailDispatcher(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor)
    {
        _repositoryManager = repositoryManager;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task DispatchEmailAsync(EmailRequestDto request)
    {
        var message = new OutboxMessage
        {
            Id = 0,
            Type = nameof(MessageTypeEnum.Email),
            Payload = JsonConvert.SerializeObject(request),
            IsProcessed = false,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = long.TryParse(_httpContext!.GetUserId(), out var createdBy) ? createdBy : 0,
            IsActive = true
        };

        await _repositoryManager.OutboxMessageRepository.AddAsync(message);
    }

    public async Task DispatchEmailAsync(List<EmailRequestDto> requests)
    {
        List<OutboxMessage> messages = new List<OutboxMessage>();

        foreach (var item in requests)
        {
            messages.Add(new OutboxMessage
            {
                Id = 0,
                Type = nameof(MessageTypeEnum.Email),
                Payload = JsonConvert.SerializeObject(item),
                IsProcessed = false,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = long.TryParse(_httpContext!.GetUserId(), out var createdBy) ? createdBy : 0,
                IsActive = true
            });
        }

        await _repositoryManager.OutboxMessageRepository.AddRangeAsync(messages);
    }
}
