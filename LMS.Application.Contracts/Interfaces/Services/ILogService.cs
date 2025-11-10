using LMS.Application.Contracts.DTOs.Log;
using LMS.Application.Contracts.DTOs;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface ILogService
{
    Task<PaginatedResponseDto<BooksLogDto>> GetBooksLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<ConfigsLogDto>> GetConfigsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<GenreLogDto>> GetGenresLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<PenaltyLogDto>> GetPenaltiesLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<MembershipLogDto>> GetMembershipsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<ReservationLogDto>> GetReservationsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<TransectionLogDto>> GetTransectionsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<UserLogDto>> GetUsersLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<UserMembershipMappingLogDto>> GetUserMembershipsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
}
