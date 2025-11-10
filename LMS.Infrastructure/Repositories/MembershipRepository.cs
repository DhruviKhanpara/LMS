using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class MembershipRepository : RepositoryBase<Membership>, IMembershipRepository
{
    public MembershipRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Membership> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).AsNoTracking().AsQueryable();
}
