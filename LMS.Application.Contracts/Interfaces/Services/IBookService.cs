using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Books;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IBookService
{
    Task<PaginatedResponseDto<T>> GetAllBookAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null, long? genreId = null) where T : class;
    Task<PaginatedResponseDto<GetBookDto>> GetAllBookByNameFilterAsync(string nameStartWith, int? pageNumber = null, int? pageSize = null, int? latestDataCount = null);
    Task<List<SelectListItem>> GetAllBookSelectionAsync(long[]? bookStatusList = null, long? bookId = null);
    Task<T> GetBookByIdAsync<T>(long id) where T : class;
    Task<GetBookInfoDto> GetBookInfoByIdAsync(long id);
    Task<byte[]> ExportBookData();
    Task AddBookAsync(AddBookDto book);
    Task UpdateBookAsync(UpdateBookDto book);
    Task DeleteBookAsync(long id);
    Task PermanentDeleteBookAsync(long id);
}
