using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IReservationRepository : IRepositoryBase<Reservation>
{
    IQueryable<Reservation> GetByIdAsync(long id);
    IQueryable<Reservation> GetReservationOrderByActiveStatus(bool? isActive = null);
}
