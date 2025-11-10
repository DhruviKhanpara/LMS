using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IPenaltyRepository : IRepositoryBase<Penalty>
{
    IQueryable<Penalty> GetByIdAsync(long id);
    IQueryable<Penalty> GetPenaltyOrderByDefault(bool? isActive = null);
}
