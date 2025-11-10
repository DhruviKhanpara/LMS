using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IUserMembershipReporsitory : IRepositoryBase<UserMembershipMapping>
{
    IQueryable<UserMembershipMapping> GetByIdAsync(long id);
    IQueryable<UserMembershipMapping> GetUserMembershipOrderByActiveStatus(bool? isActive = null);
}
