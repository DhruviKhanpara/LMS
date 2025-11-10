using LMS.Application.Contracts.DTOs.Transection;
using LMS.Application.Contracts.DTOs;
using LMS.Core.Enums;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface ITransectionService
{
    Task<PaginatedResponseDto<T>> GetAllTransectionAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? userId = null, long? bookId = null) where T : class;
    Task<PaginatedResponseDto<GetUserTransectionDto>> GetUserTransectionAsync(int? pageSize = null, int? pageNumber = null);
    Task<T> GetTransectionByIdAsync<T>(long id) where T : class;
    Task BorrowBookforLoginUserAsync(long bookId, long? userId = null);
    Task<byte[]> ExportTransectionData();
    Task AddTransectionAsync(AddTransectionDto transection);
    Task UpdateTransectionAsync(UpdateTransectionDto transection);
    Task TransectionActionsAsync(long id, TransectionActionEnum transectionAction);
    Task PermanentDeleteTransectionAsync(long id);
    Task DispatchEmailForDueDateRemainder();
}
