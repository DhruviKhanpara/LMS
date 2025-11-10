using LMS.Core.Entities;

namespace LMS.Application.Contracts.Interfaces.Repositories;

public interface IConfigRepository : IRepositoryBase<Configs>
{
    IQueryable<Configs> GetByIdAsync(long id);
    IQueryable<Configs> GetByKeyNameAsync(string keyName);
    IQueryable<Configs> GetByKeyNameListAsync(List<string> keyNames);
}
