using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage message);
    Task AddRangeAsync(List<OutboxMessage> messages);
    Task<List<OutboxMessage>> GetUnprocessedAsync();
    Task UpdateAsync(OutboxMessage message);
}
