using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Genre;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IGenreService
{
    Task<PaginatedResponseDto<T>> GetAllGenreAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class;
    Task<List<SelectListItem>> GetAllGenreSelectionAsync();
    Task<T> GetGenreByIdAsync<T>(long id) where T : class;
    Task<byte[]> ExportGenreData();
    Task AddGenreAsync(AddGenreDto genre);
    Task UpdateGenreAsync(UpdateGenreDto genre);
    Task DeleteGenreAsync(long id);
    Task PermanentDeleteGenreAsync(long id);
}
