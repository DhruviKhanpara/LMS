using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IConfigsService
{
    Task<PaginatedResponseDto<T>> GetAllConfigsAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class;
    Task<List<GetConfigsValueDto>> GetAllConfigValues();
    Task<List<SelectListItem>> GetAllConfigsSelectionAsync();
    Task<T> GetConfigsByIdAsync<T>(long id) where T : class;
    Task<List<GetConfigsValueDto>> GetConfigsByKeyName(List<string> keyNames);
    Task<byte[]> ExportConfigData();
    Task UpdateConfigs(UpdateConfigsDto configs);
    Task<List<JobSchedule>> GetAllSchedules(string? jobName = null);
}
