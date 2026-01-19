using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Log;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Core.Common;
using LMS.Core.Entities.LogEntities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Application.Services.Services;

internal class LogService : ILogService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly HttpContext? _httpContext;
    private readonly IMapper _mapper;

    public LogService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        _repositoryManager = repositoryManager;
        _httpContext = httpContextAccessor.HttpContext;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<BooksHistoryDto>> GetBooksLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var booksLogQuery = _repositoryManager.BooksLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<BooksLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await booksLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var booksLog = await booksLogQuery
            .ProjectTo<BooksHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<BooksHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<BooksHistoryDto>()
        {
            Pagination = paginationModel,
            Data = booksLog
        };
    }

    public async Task<PaginatedResponseDto<ConfigsHistoryDto>> GetConfigsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var configsLogQuery = _repositoryManager.ConfigsLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<ConfigsLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await configsLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var configsLog = await configsLogQuery
            .ProjectTo<ConfigsHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<ConfigsHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<ConfigsHistoryDto>()
        {
            Pagination = paginationModel,
            Data = configsLog
        };
    }

    public async Task<PaginatedResponseDto<GenreHistoryDto>> GetGenresLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        var genreLogQuery = _repositoryManager.GenreLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<GenreLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await genreLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var genreLog = await genreLogQuery
            .ProjectTo<GenreHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<GenreHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<GenreHistoryDto>()
        {
            Pagination = paginationModel,
            Data = genreLog
        };
    }

    public async Task<PaginatedResponseDto<PenaltyHistoryDto>> GetPenaltiesLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var penaltyLogQuery = _repositoryManager.PenaltyLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<PenaltyLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await penaltyLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var penaltyLog = await penaltyLogQuery
            .ProjectTo<PenaltyHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<PenaltyHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<PenaltyHistoryDto>()
        {
            Pagination = paginationModel,
            Data = penaltyLog
        };
    }

    public async Task<PaginatedResponseDto<MembershipHistoryDto>> GetMembershipsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        var membershipLogQuery = _repositoryManager.MembershipLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<MembershipLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await membershipLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var membershipLog = await membershipLogQuery
            .ProjectTo<MembershipHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<MembershipHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<MembershipHistoryDto>()
        {
            Pagination = paginationModel,
            Data = membershipLog
        };
    }

    public async Task<PaginatedResponseDto<ReservationHistoryDto>> GetReservationsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var reservationLogQuery = _repositoryManager.ReservationLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<ReservationLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await reservationLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var reservationLog = await reservationLogQuery
            .ProjectTo<ReservationHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<ReservationHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<ReservationHistoryDto>()
        {
            Pagination = paginationModel,
            Data = reservationLog
        };
    }

    public async Task<PaginatedResponseDto<TransectionHistoryDto>> GetTransectionsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var transectionLogQuery = _repositoryManager.TransectionLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<TransectionLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await transectionLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var transectionLog = await transectionLogQuery
            .ProjectTo<TransectionHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<TransectionHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<TransectionHistoryDto>()
        {
            Pagination = paginationModel,
            Data = transectionLog
        };
    }

    public async Task<PaginatedResponseDto<UserHistoryDto>> GetUsersLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var userLogQuery = _repositoryManager.UserLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<UserLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await userLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var userLog = await userLogQuery
            .ProjectTo<UserHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<UserHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<UserHistoryDto>()
        {
            Pagination = paginationModel,
            Data = userLog
        };
    }

    public async Task<PaginatedResponseDto<UserMembershipMappingHistoryDto>> GetUserMembershipsLogAsync(long? id = null, int? pageSize = null, int? pageNumber = null, string? orderBy = null, string? logOperation = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, long? performedByUserId = null)
    {
        var authUserRole = _httpContext!.GetUserRole();

        if (!authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Only Admin can access this");

        startDate = startDate ?? DateTimeOffset.UtcNow.AddMonths(-1);
        endDate = endDate ?? DateTimeOffset.UtcNow;

        var userMembershipMappingLogQuery = _repositoryManager.UserMembershipMappingLogRepository
            .GetAllAsync()
            .Where(x => (!id.HasValue || x.Id == id.Value) &&
                (logOperation == null || (x.Operation != null && x.Operation.ToLower() == logOperation.ToLower())) &&
                (!startDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date >= startDate.Value.Date)) &&
                (!endDate.HasValue || (x.LogTime.HasValue && x.LogTime.Value.Date <= endDate.Value.Date)))
            .Where(BuildUserOperationFilter<UserMembershipMappingLog>(performedByUserId))
            .OrderByDescending(x => x.LogTime)
            .ThenByDescending(x => x.Id);

        var totalCount = await userMembershipMappingLogQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var userMembershipMappingLog = await userMembershipMappingLogQuery
            .ProjectTo<UserMembershipMappingHistoryDto>(_mapper.ConfigurationProvider)
            .Sort<UserMembershipMappingHistoryDto>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<UserMembershipMappingHistoryDto>()
        {
            Pagination = paginationModel,
            Data = userMembershipMappingLog
        };
    }

    #region Private section
    private static Expression<Func<T, bool>> BuildUserOperationFilter<T>(long? performedByUserId = null) where T : LogAudit
    {
        if (!performedByUserId.HasValue)
            return x => true;

        return x =>
            x.Operation != null &&
            (
                x.Operation.ToLower() == LogOperationTypes.Insert.ToLower() && x.CreatedBy == performedByUserId ||
                (x.Operation.ToLower() == LogOperationTypes.Update.ToLower() || x.Operation.ToLower() == LogOperationTypes.Restore.ToLower()) && x.ModifiedBy == performedByUserId ||
                (x.Operation.ToLower() == LogOperationTypes.SoftDelete.ToLower() || x.Operation.ToLower() == LogOperationTypes.Delete.ToLower()) && x.DeletedBy == performedByUserId
            );
    }
    #endregion
}
