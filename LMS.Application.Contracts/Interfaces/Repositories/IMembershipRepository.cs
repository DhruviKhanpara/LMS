using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IMembershipRepository : IRepositoryBase<Membership>
{
    IQueryable<Membership> GetByIdAsync(long id);
}
