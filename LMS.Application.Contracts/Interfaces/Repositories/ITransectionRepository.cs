using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface ITransectionRepository : IRepositoryBase<Transection>
{
    IQueryable<Transection> GetByIdAsync(long id);
    IQueryable<Transection> GetTransectionOrderByActiveStatus(bool? isActive = null);
}
