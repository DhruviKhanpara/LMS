using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Repositories;

internal class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly LibraryManagementSysContext _context;
    private readonly DbSet<T> _dbSet;

    public RepositoryBase(LibraryManagementSysContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> GetAllAsync(bool? isActive = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        if (isActive.HasValue)
        {
            query = query.Where(e => EF.Property<bool>(e, "IsActive") == isActive.Value);
        }

        return query.AsQueryable();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => _dbSet.Where(expression).AsNoTracking().AsQueryable();

    public Task<bool> AnyAsync(Expression<Func<T, bool>> expression) => _dbSet.AnyAsync(expression);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void UpdateRange(List<T> entities) => _dbSet.UpdateRange(entities);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public void RemoveRange(List<T> entities) => _dbSet.RemoveRange(entities);
}
