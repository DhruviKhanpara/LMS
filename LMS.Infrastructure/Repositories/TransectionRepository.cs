using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class TransectionRepository : RepositoryBase<Transection>, ITransectionRepository
{
    public TransectionRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Transection> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).Include(x => x.Book).AsNoTracking().AsQueryable();

    public IQueryable<Transection> GetTransectionOrderByActiveStatus(bool? isActive = null) => GetAllAsync(isActive: isActive)
        .OrderBy(x => x.IsActive == false ? 1 : 0)
        .ThenBy(x => x.ReturnDate == null ? 0 : 1)
        .ThenByDescending(x => x.ReturnDate)
        .ThenBy(x => x.LostClaimDate == null ? 0 : 1)
        .ThenByDescending(x => x.LostClaimDate);
}
