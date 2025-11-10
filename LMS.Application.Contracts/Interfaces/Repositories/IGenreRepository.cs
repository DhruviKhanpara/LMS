using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IGenreRepository : IRepositoryBase<Genre>
{
    IQueryable<Genre> GetByIdAsync(long id);
}
