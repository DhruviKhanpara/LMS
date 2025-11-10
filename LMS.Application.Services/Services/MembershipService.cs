using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Membership;
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

internal class MembershipService : IMembershipService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly HttpContext? _httpContext;

    public MembershipService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _validationService = validationService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<PaginatedResponseDto<T>> GetAllMembershipAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class
    {
        bool isLogin = long.TryParse(_httpContext?.GetUserId(), out long authUserId);

        var membershipQuery = _repositoryManager.MembershipRepository
            .GetAllAsync(isActive: _httpContext!.GetUserRole().Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true);

        var totalCount = await membershipQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var memberships = await membershipQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = memberships
        };
    }

    public async Task<List<SelectListItem>> GetAllMembershipSelectionAsync()
    {
        var membershipSelection = await _repositoryManager.MembershipRepository.GetAllAsync(isActive: true)
            .ProjectTo<SelectListItem>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return membershipSelection ?? new List<SelectListItem>();
    }

    public async Task<T> GetMembershipByIdAsync<T>(long id) where T : class
    {
        var membership = await _repositoryManager.MembershipRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return membership ?? Activator.CreateInstance<T>();
    }

    public async Task<byte[]> ExportMembershipData()
    {
        var memberships = await GetAllMembershipAsync<ExportMembershipDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Memberships", memberships.Data } });
    }

    public async Task AddMembershipAsync(AddMembershipDto membership)
    {
        _validationService.Validate<AddMembershipDto>(membership);

        var mappedMembership = _mapper.Map<Membership>(membership);
        await _repositoryManager.MembershipRepository.AddAsync(mappedMembership);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdateMembershipAsync(UpdateMembershipDto membership)
    {
        _validationService.Validate<UpdateMembershipDto>(membership);

        var existMembership = await _repositoryManager.MembershipRepository.GetByIdAsync(id: membership.Id).FirstOrDefaultAsync();

        if (existMembership == null)
            throw new BadRequestException("This Membership is not available");

        var mappedMembership = _mapper.Map(membership, existMembership);

        _repositoryManager.MembershipRepository.Update(entity: mappedMembership);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task DeleteMembershipAsync(long id)
    {
        if (!_httpContext!.GetUserRole().Equals(nameof(RoleListEnum.Admin), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("You haven't permission to proceed");

        var existMembership = await _repositoryManager.MembershipRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existMembership == null)
            throw new BadRequestException("This Membership is already not exist");

        existMembership.IsActive = false;

        _repositoryManager.MembershipRepository.Update(entity: existMembership);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task PermanentDeleteMembershipAsync(long id)
    {
        var existMembership = await _repositoryManager.MembershipRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existMembership == null)
            throw new BadRequestException("This Membership is already not exist");

        _repositoryManager.MembershipRepository.Remove(entity: existMembership);
        _repositoryManager.UnitOfWork.SaveChanges();
    }
}
