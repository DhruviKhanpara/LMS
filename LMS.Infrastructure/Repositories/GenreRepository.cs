using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class GenreRepository : RepositoryBase<Genre>, IGenreRepository
{
    public GenreRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Genre> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).AsNoTracking().AsQueryable();
}
