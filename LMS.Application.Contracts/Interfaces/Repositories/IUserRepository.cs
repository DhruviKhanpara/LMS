using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    IQueryable<User> GetByIdAsync(long id);
    Task<bool> IsUserExistenceAsync(string username, string email, long roleId, long? userId = null);
}
