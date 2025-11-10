using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Core.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

internal class ConfigRepository : RepositoryBase<Configs>, IConfigRepository
{
    public ConfigRepository(LibraryManagementSysContext context) : base(context) { }

    public IQueryable<Configs> GetByIdAsync(long id) => FindByCondition(x => x.Id == id && x.IsActive).AsNoTracking().AsQueryable();

    public IQueryable<Configs> GetByKeyNameAsync(string keyName)
    {
        return FindByCondition(x => x.KeyName.ToLower() == keyName.ToLower() && x.IsActive).AsNoTracking().AsQueryable();
    }

    public IQueryable<Configs> GetByKeyNameListAsync(List<string> keyNames)
    {
        return FindByCondition(x => keyNames.Any(y => y.ToLower() == x.KeyName.ToLower()) && x.IsActive).AsNoTracking().AsQueryable();
    }
}
