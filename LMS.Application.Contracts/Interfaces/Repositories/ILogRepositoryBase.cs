using System.Linq.Expressions;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface ILogRepositoryBase<T> where T : class
{
    IQueryable<T> GetAllAsync(bool? isActive = null);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
}
