using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly LibraryManagementSysContext _context;

    public OutboxMessageRepository(LibraryManagementSysContext context) => _context = context;

    public async Task AddAsync(OutboxMessage message)
        => await _context.OutboxMessages.AddAsync(message);

    public async Task AddRangeAsync(List<OutboxMessage> messages)
        => await _context.OutboxMessages.AddRangeAsync(messages);

    public async Task<List<OutboxMessage>> GetUnprocessedAsync()
        => await _context.OutboxMessages
            .Where(x => !x.IsProcessed && (x.NextAttemptAt == null || x.NextAttemptAt <= DateTimeOffset.UtcNow) && x.IsActive)
            .ToListAsync();

    public async Task UpdateAsync(OutboxMessage message)
    {
        _context.OutboxMessages.Update(message);
        await _context.SaveChangesAsync();
    }
}
