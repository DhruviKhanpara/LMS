using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class BookRepository : RepositoryBase<Books>, IBookRepository
{
    public BookRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Books> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).Include(x => x.BookFileMappings.Where(e => e.IsActive)).AsNoTracking().AsQueryable();
}
