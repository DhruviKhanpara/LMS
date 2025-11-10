using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<User> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).AsNoTracking().AsQueryable();

    public async Task<bool> IsUserExistenceAsync(string username, string email, long roleId, long? userId = null)
    {
        return
            await AnyAsync(x => (userId == null || x.Id != userId) &&
                x.Username.ToLower() == username.ToLower() &&
                x.Email.ToLower() == email.ToLower() &&
                x.RoleId == roleId &&
                x.IsActive);
    }
}