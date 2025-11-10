using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Repositories;

internal class LogRepositoryBase<T> : ILogRepositoryBase<T> where T : class
{
    protected readonly LibraryManagementSysContext _context;
    private readonly DbSet<T> _dbSet;

    public LogRepositoryBase(LibraryManagementSysContext context)
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
}
