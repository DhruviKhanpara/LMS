using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IUserMembershipMappingService
{
    Task<PaginatedResponseDto<T>> GetUserMembershipAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null) where T : class;
    Task<List<GetUserMembersipListDto>> GetLoginUserMembershipAsync();
    Task<T> GetUserMembershipByIdAsync<T>(long id) where T : class;
    Task<Dictionary<string, bool>> GetUserEligibilityToAdd();
    Task<byte[]> ExportUserMembershipData();
    Task AddOrUpgradeUserMembershipAsync(long membershipId, bool IsUpgradePlan = false, long? userId = null);
    Task UpdateUserMembershipAsync(UpdateUserMembershipDto userMembership);
    Task DeleteUserMembershipAsync(long id);
    Task DeleteAllUserMembershipAsync(long userId);
    Task PermanentDeleteUserMembershipAsync(long id);
    Task RemoveExpiredUserMembershipAsync(bool forLogin = false);
    Task DispatchEmailForMembershipDueRemainder();
}
