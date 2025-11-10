using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Core.Enums;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LMS.Infrastructure.Repositories;

internal class PenaltyRepository : RepositoryBase<Penalty>, IPenaltyRepository
{
    public PenaltyRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Penalty> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).AsNoTracking().AsQueryable();

    public IQueryable<Penalty> GetPenaltyOrderByDefault(bool? isActive = null) => GetAllAsync(isActive: isActive)
        .OrderBy(x => x.IsActive == false ? 1 : 0)
        .ThenBy(x => x.StatusId == (long)FineStatusEnum.UnPaid ? 0 : x.StatusId == (long)FineStatusEnum.Paid ? 1 : 2)
        .ThenByDescending(x => x.ModifiedAt);
}
