using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Application.Services.Constants;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace LMS.Application.Services.Servicesl;

internal class ConfigsService : IConfigsService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IValidationService _validationService;
    private readonly HttpContext? _httpContext;
    private readonly IMapper _mapper;

    public ConfigsService(IRepositoryManager repositoryManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService)
    {
        _repositoryManager = repositoryManager;
        _validationService = validationService;
        _mapper = mapper;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<PaginatedResponseDto<T>> GetAllConfigsAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        var configsQuery = _repositoryManager.ConfigRepository
            .GetAllAsync(isActive: authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true);

        var totalCount = await configsQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var configs = await configsQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = configs
        };
    }

    public async Task<List<GetConfigsValueDto>> GetAllConfigValues()
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var configs = await _repositoryManager.ConfigRepository
            .GetAllAsync(isActive: true)
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var userMembershipLimits = await _repositoryManager.UserMembershipMappingRepository
            .FindByCondition(x => x.UserId == authUserId && x.IsActive && x.EffectiveStartDate < DateTimeOffset.UtcNow && x.ExpirationDate > DateTimeOffset.UtcNow)
            .Select(x => new
            {
                BorrowLimit = x.BorrowLimit,
                ReservationLmit = x.ReservationLimit
            })
            .FirstOrDefaultAsync();

        configs.Add(new GetConfigsValueDto()
        {
            KeyName = ConfigKeysConstants.BorrowLimit,
            KeyValue = userMembershipLimits?.BorrowLimit.ToString() ?? ""
        });

        configs.Add(new GetConfigsValueDto()
        {
            KeyName = ConfigKeysConstants.ReservationLimit,
            KeyValue = userMembershipLimits?.ReservationLmit.ToString() ?? ""
        });

        return configs ?? new List<GetConfigsValueDto>();
    }

    public async Task<List<SelectListItem>> GetAllConfigsSelectionAsync()
    {
        var configSelection = await _repositoryManager.ConfigRepository.GetAllAsync(isActive: true)
            .ProjectTo<SelectListItem>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return configSelection ?? new List<SelectListItem>();
    }

    public async Task<T> GetConfigsByIdAsync<T>(long id) where T : class
    {
        var configs = await _repositoryManager.ConfigRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return configs ?? Activator.CreateInstance<T>();
    }

    public async Task<List<GetConfigsValueDto>> GetConfigsByKeyName(List<string> keyNames)
    {
        var configs = await _repositoryManager.ConfigRepository
            .GetByKeyNameListAsync(keyNames: keyNames)
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return configs;
    }

    public async Task<byte[]> ExportConfigData()
    {
        var configs = await GetAllConfigsAsync<ExportConfigDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "Configs", configs.Data } });
    }

    public async Task UpdateConfigs(UpdateConfigsDto configs)
    {
        _validationService.Validate<UpdateConfigsDto>(configs);

        var existConfigs = await _repositoryManager.ConfigRepository.GetByIdAsync(id: configs.Id).FirstOrDefaultAsync();

        if (existConfigs == null)
            throw new BadRequestException("This Configs is not available");

        existConfigs.KeyValue = configs.KeyValue;
        existConfigs.Description = configs.Description;

        _repositoryManager.ConfigRepository.Update(existConfigs);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task<List<JobSchedule>> GetAllSchedules(string? jobName = null)
    {
        var configs = await _repositoryManager.ConfigRepository
            .GetAllAsync(isActive: true)
            .Where(x => x.KeyName.Contains("_")
            && (jobName == null || x.KeyName.ToLower().Contains(jobName.ToLower())))
            .ToListAsync();

        var grouped = configs
            .GroupBy(kvp => kvp.KeyName.Split('_')[0]) // Group by job name prefix
            .ToDictionary(g => g.Key, g => g.ToDictionary(kvp => kvp.KeyName, kvp => kvp.KeyValue));

        var schedules = new List<JobSchedule>();

        foreach (var job in grouped.Keys)
        {
            var config = grouped[job];

            schedules.Add(new JobSchedule
            {
                Name = job,
                FrequencyType = config.GetValueOrDefault($"{job}_Frequency") ?? string.Empty,
                Interval = int.TryParse(config.GetValueOrDefault($"{job}_Interval"), out var interval) ? interval : null,
                Time = config.GetValueOrDefault($"{job}_Time")
            });
        }

        return schedules;
    }
}
