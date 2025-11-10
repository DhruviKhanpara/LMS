using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class UserMembershipReporsitory : RepositoryBase<UserMembershipMapping>, IUserMembershipReporsitory
{
    public UserMembershipReporsitory(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<UserMembershipMapping> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).AsNoTracking().AsQueryable();

    public IQueryable<UserMembershipMapping> GetUserMembershipOrderByActiveStatus(bool? isActive = null) => GetAllAsync(isActive: isActive)
        .OrderBy(x => x.IsActive == false ? 1 : 0)
        .ThenBy(x => x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow ? 0 : x.EffectiveStartDate > DateTimeOffset.UtcNow ? 1 : 2)
        .ThenBy(x => x.EffectiveStartDate)
        .ThenBy(x => x.ExpirationDate)
        .ThenByDescending(x => x.BorrowLimit)
        .ThenByDescending(x => x.ReservationLimit);
}
