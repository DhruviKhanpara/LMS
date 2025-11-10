using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Membership;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IMembershipService
{
    Task<PaginatedResponseDto<T>> GetAllMembershipAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class;
    Task<List<SelectListItem>> GetAllMembershipSelectionAsync();
    Task<T> GetMembershipByIdAsync<T>(long id) where T : class;
    Task<byte[]> ExportMembershipData();
    Task AddMembershipAsync(AddMembershipDto membership);
    Task UpdateMembershipAsync(UpdateMembershipDto membership);
    Task DeleteMembershipAsync(long id);
    Task PermanentDeleteMembershipAsync(long id);
}
