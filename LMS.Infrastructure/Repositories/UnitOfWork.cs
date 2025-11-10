using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Common.Helpers;
using LMS.Core.Common;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace LMS.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly LibraryManagementSysContext _context;
    private readonly HttpContext? _httpContext;
    private IDbContextTransaction _currentTransaction;

    public UnitOfWork(LibraryManagementSysContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContext = httpContextAccessor?.HttpContext;
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction == null)
            return;

        await _currentTransaction.RollbackAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _currentTransaction.CommitAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public int SaveChanges()
    {
        var entries = UpdateAuditInEntityBeforeSave();

        int result = _context.SaveChanges();

        if (entries.Any())
        {
            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                {
                    if (user.CreatedBy == 0 || user.CreatedBy == null)
                    {
                        user.CreatedBy = user.Id;
                    }
                }
            }

            _context.SaveChanges();
        }

        return result;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = UpdateAuditInEntityBeforeSave();

        int result = await _context.SaveChangesAsync(cancellationToken);

        if (entries.Any())
        {
            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                {
                    if (user.CreatedBy == 0 || user.CreatedBy == null)
                    {
                        user.CreatedBy = user.Id;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    private IEnumerable<EntityEntry> UpdateAuditInEntityBeforeSave()
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var entities = _context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is Audit && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var props = entity.Metadata.GetProperties().Select(p => p.Name).ToHashSet();

            if (entity.State == EntityState.Added)
            {
                if (props.Contains("CreatedBy"))
                    entity.Property("CreatedBy").CurrentValue = isLogin ? authUserId : null;
            }
            else if ((props.Contains("IsActive") && entity.Property("IsActive").CurrentValue as bool? == false) || entity.State == EntityState.Deleted)
            {
                if (props.Contains("DeletedBy"))
                    entity.Property("DeletedBy").CurrentValue = isLogin ? authUserId : null;

                if (props.Contains("DeletedAt"))
                    entity.Property("DeletedAt").CurrentValue = DateTimeOffset.UtcNow;
            }
            else if (entity.State == EntityState.Modified)
            {
                if (props.Contains("ModifiedBy"))
                    entity.Property("ModifiedBy").CurrentValue = isLogin ? authUserId : null;

                if (props.Contains("ModifiedAt"))
                    entity.Property("ModifiedAt").CurrentValue = DateTimeOffset.UtcNow;
            }
        }

        return entities.ToList();
    }
}
