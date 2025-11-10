using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IBookRepository : IRepositoryBase<Books>
{
    IQueryable<Books> GetByIdAsync(long id);
}
