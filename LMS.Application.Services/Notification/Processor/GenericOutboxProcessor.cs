using LMS.Application.Contracts.Interfaces.Notification;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Services.Constants;
using LMS.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Services.Notification.Processor;

internal class GenericOutboxProcessor : IOutboxProcessor
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IEnumerable<IOutboxMessageHandler> _handlers;
    private readonly ILogger<GenericOutboxProcessor> _logger;

    public GenericOutboxProcessor(IRepositoryManager repositoryManager, IEnumerable<IOutboxMessageHandler> handlers, ILogger<GenericOutboxProcessor> logger)
    {
        _repositoryManager = repositoryManager;
        _handlers = handlers;
        _logger = logger;
    }

    public async Task ProcessAsync()
    {
        var messages = await _repositoryManager.OutboxMessageRepository.GetUnprocessedAsync();

        var retryCount = await GetRetryCount();
        var retryDelayMinute = await GetRetryDelayMinutes();

        foreach (var message in messages)
        {
            var handler = _handlers.FirstOrDefault(x => x.MessageType == message.Type);
            if (handler == null)
            {
                _logger.LogWarning($"[OUTBOX] No handler for type {message.Type}");
                continue;
            }

            try
            {
                await handler.HandleAsync(message);
                message.IsProcessed = true;
                message.ProcessedAt = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {
                if (message.RetryCount >= retryCount)
                {
                    message.IsProcessed = true;
                    message.ProcessedAt = DateTime.UtcNow;
                    _logger.LogWarning($"[OUTBOX] Message {message.Type} failed after max retries. Marking as processed.");
                }
                else
                {
                    message.RetryCount++;
                    message.NextAttemptAt = DateTimeOffset.UtcNow.Add(
                        RetryHelper.GetExponentialBackoff(message.RetryCount, retryDelayMinute)
                    );
                }

                _logger.LogError($"[OUTBOX] Error processing message. Type={message.Type}, Error={ex.Message}");
            }

            await _repositoryManager.OutboxMessageRepository.UpdateAsync(message);
        }
    }

    private async Task<int> GetRetryDelayMinutes()
    {
        int.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync(ConfigKeysConstants.OutboxMaxRetryDelayMinutes).FirstOrDefaultAsync())?.KeyValue ?? "0", out int retryDelayMinues);
        return retryDelayMinues;
    }

    private async Task<int> GetRetryCount()
    {
        int.TryParse((await _repositoryManager.ConfigRepository.GetByKeyNameAsync(ConfigKeysConstants.OutboxMaxRetryCount).FirstOrDefaultAsync())?.KeyValue ?? "0", out int retryCount);
        return retryCount;
    }
}
