namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task RollbackTransactionAsync();
    Task CommitTransactionAsync();
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
