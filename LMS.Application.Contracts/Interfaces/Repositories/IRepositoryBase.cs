using System.Linq.Expressions;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IRepositoryBase<T> where T : class
{
    IQueryable<T> GetAllAsync(bool? isActive = null);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
	Task AddAsync(T entity);
    void Update(T entity);
    void UpdateRange(List<T> entities);
    void Remove(T entity);
    void RemoveRange(List<T> entities);
}
