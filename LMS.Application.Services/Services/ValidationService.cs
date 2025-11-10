using FluentValidation;
using FluentValidation.Results;
using LMS.Application.Contracts.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LMS.Application.Services.Services;

internal class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ValidationService> _logger;
    private readonly MemoryCache _cache;
    private readonly Dictionary<Type, bool> _validationSettings = new();

    private bool _isValidationDisabledGlobally = false;

    public ValidationService(IServiceProvider serviceProvider, ILogger<ValidationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public void SetGlobalValidation(bool isEnabled)
    {
        _isValidationDisabledGlobally = !isEnabled;
    }

    public void EnableValidation<T>(bool isEnabled) where T : class => _validationSettings[typeof(T)] = isEnabled;

    public void Validate<T>(T dto) where T : class
    {
        if (_isValidationDisabledGlobally || (_validationSettings.TryGetValue(typeof(T), out bool isEnabled) && !isEnabled))
            return;

        string cacheKey = $"{typeof(T).FullName}:{JsonConvert.SerializeObject(dto)}";

        if (_cache.TryGetValue(cacheKey, out ValidationResult cachedResult))
        {
            if (!cachedResult.IsValid) throw new ValidationException(cachedResult.Errors);
            return;
        }

        var validator = _serviceProvider.GetServices<IValidator<T>>().FirstOrDefault();
        if (validator == null)
        {
            _logger.LogWarning("No validator found for {DtoType}. Skipping validation.", typeof(T).Name);
            return;
        }

        ValidationResult result = validator.Validate(dto);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        if (!result.IsValid) throw new CustomValidationException(result.Errors);
    }

    public async Task ValidateAsync<T>(T dto, CancellationToken cancellationToken = default) where T : class
    {
        if (_isValidationDisabledGlobally || (_validationSettings.TryGetValue(typeof(T), out bool isEnabled) && !isEnabled))
            return;

        string cacheKey = $"{typeof(T).FullName}:{dto.GetHashCode()}";
        if (_cache.TryGetValue(cacheKey, out ValidationResult cachedResult))
        {
            if (!cachedResult.IsValid) throw new ValidationException(cachedResult.Errors);
            return;
        }

        var validator = _serviceProvider.GetServices<IValidator<T>>().FirstOrDefault();
        if (validator == null)
        {
            _logger.LogWarning("No validator found for {DtoType}. Skipping validation.", typeof(T).Name);
            return;
        }

        ValidationResult result = await validator.ValidateAsync(dto, cancellationToken);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        if (!result.IsValid) throw new CustomValidationException(result.Errors);
    }

    public void ValidateAll<T>(IEnumerable<T> dtos) where T : class
    {
        foreach (var dto in dtos)
            Validate(dto);
    }

    public async Task ValidateAllAsync<T>(IEnumerable<T> dtos, CancellationToken cancellationToken = default) where T : class
    {
        var validationTasks = dtos.Select(dto => ValidateAsync(dto, cancellationToken));
        await Task.WhenAll(validationTasks);
    }
}
