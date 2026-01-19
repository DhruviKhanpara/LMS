using LMS.Application.Contracts.DTOs.Log;
using LMS.Application.Contracts.DTOs;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface ILogService
{
    Task<PaginatedResponseDto<BooksHistoryDto>> GetBooksLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<ConfigsHistoryDto>> GetConfigsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<GenreHistoryDto>> GetGenresLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<PenaltyHistoryDto>> GetPenaltiesLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<MembershipHistoryDto>> GetMembershipsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<ReservationHistoryDto>> GetReservationsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<TransectionHistoryDto>> GetTransectionsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<UserHistoryDto>> GetUsersLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
    Task<PaginatedResponseDto<UserMembershipMappingHistoryDto>> GetUserMembershipsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null);
}
