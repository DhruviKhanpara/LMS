using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Core.Enums;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
{
    public ReservationRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Reservation> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).Include(x => x.Book).AsNoTracking().AsQueryable();

    public IQueryable<Reservation> GetReservationOrderByActiveStatus(bool? isActive = null) => GetAllAsync(isActive: isActive)
        .OrderBy(x => x.IsActive == false ? 2 : x.CancelDate != null ? 1 : 0)
        .ThenBy(x => x.StatusId == (long)ReservationsStatusEnum.Allocated ? 0 : x.StatusId == (long)ReservationsStatusEnum.Reserved ? 1 : x.StatusId == (long)ReservationsStatusEnum.Fulfilled ? 2 : 3)
        .ThenBy(x => x.AllocatedAt == null ? 0 : 1)
        .ThenByDescending(x => x.AllocatedAt)
        .ThenByDescending(x => x.AllocateAfter);
}
