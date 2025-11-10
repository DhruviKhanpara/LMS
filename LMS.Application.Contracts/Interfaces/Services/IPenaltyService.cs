using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Contracts.DTOs;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IPenaltyService
{
    Task<PaginatedResponseDto<T>> GetAllPenaltyAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null, long? penaltyTypeId = null) where T : class;
    Task<PaginatedResponseDto<GetPenaltyDto>> GetUserPenaltyAsync(int ? pageSize = null, int? pageNumber = null);
    Task<T> GetPenaltyByIdAsync<T>(long id) where T : class;
    Task<bool> HavePendingPenalty();
    Task<byte[]> ExportPenaltyData();
    Task AddPenaltyAsync(AddPenaltyDto penalty);
    Task UpdatePenaltyAsync(UpdatePenaltyDto penalty);
    Task RemovePenaltyAsync(long id);
    Task PermanentDeletePenaltyAsync(long id);
    Task CalculatePenaltyForHoldingBooks(bool forLogin = false);
}
