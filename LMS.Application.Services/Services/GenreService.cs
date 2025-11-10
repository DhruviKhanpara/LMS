using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Genre;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class GenreService : IGenreService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly HttpContext? _httpContext;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public GenreService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _httpContext = httpContextAccessor.HttpContext;
        _validationService = validationService;
    }

    public async Task<PaginatedResponseDto<T>> GetAllGenreAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class
    {
        var genreQuery = _repositoryManager.GenreRepository
            .GetAllAsync(isActive: _httpContext!.GetUserRole().Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true);

        var totalCount = await genreQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var genres = await genreQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = genres
        };
    }

    public async Task<List<SelectListItem>> GetAllGenreSelectionAsync()
    {
        var genreSelection = await _repositoryManager.GenreRepository.GetAllAsync(isActive: true)
            .ProjectTo<SelectListItem>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return genreSelection ?? new List<SelectListItem>();
    }

    public async Task<T> GetGenreByIdAsync<T>(long id) where T : class
    {
        var genre = await _repositoryManager.GenreRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return genre ?? Activator.CreateInstance<T>();
    }

    public async Task<byte[]> ExportGenreData()
    {
        var genres = await GetAllGenreAsync<ExportGenreDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Genres", genres.Data } });
    }

    public async Task AddGenreAsync(AddGenreDto genre)
    {
        _validationService.Validate<AddGenreDto>(genre);

        var exist = await _repositoryManager.GenreRepository.AnyAsync(x => x.IsActive && x.Name.ToLower() == genre.Name.ToLower());
        if (exist)
            throw new BadRequestException("Genre with same name exist");

        var mappedGenre = _mapper.Map<Genre>(genre);
        await _repositoryManager.GenreRepository.AddAsync(mappedGenre);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdateGenreAsync(UpdateGenreDto genre)
    {
        _validationService.Validate<UpdateGenreDto>(genre);

        var existGenre = await _repositoryManager.GenreRepository.GetByIdAsync(id: genre.Id).FirstOrDefaultAsync();

        if (existGenre == null)
            throw new BadRequestException("This Genre is not available");

        var exist = await _repositoryManager.GenreRepository.AnyAsync(x => x.IsActive && x.Name.ToLower() == genre.Name.ToLower() && x.Id != genre.Id);
        if (exist)
            throw new BadRequestException("Genre with same name exist");

        var mappedGenre = _mapper.Map(genre, existGenre);

        _repositoryManager.GenreRepository.Update(entity: mappedGenre);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task DeleteGenreAsync(long id)
    {
        var existGenre = await _repositoryManager.GenreRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existGenre == null)
            throw new BadRequestException("This Genre is already not exist");

        var exist = await _repositoryManager.GenreRepository.AnyAsync(x => x.IsActive && x.Id == id && x.Books.Any(y => y.IsActive));
        if (exist)
            throw new BadRequestException("Genre is in use can't remove this");

        existGenre.IsActive = false;

        _repositoryManager.GenreRepository.Update(entity: existGenre);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task PermanentDeleteGenreAsync(long id)
    {
        var existGenre = await _repositoryManager.GenreRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existGenre == null)
            throw new BadRequestException("This Genre is already not exist");

        _repositoryManager.GenreRepository.Remove(entity: existGenre);
        _repositoryManager.UnitOfWork.SaveChanges();
    }
}
