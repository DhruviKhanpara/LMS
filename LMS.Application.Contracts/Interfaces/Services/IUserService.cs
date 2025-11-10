using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IUserService
{
    Task<PaginatedResponseDto<T>> GetAllUserAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = true) where T : class;
    Task<List<SelectListItem>> GetAllUserSelectionAsync(long[]? userRoleList = null, long? userId = null);
    Task<T> GetUserByIdAsync<T>(long id) where T : class;
    Task<UserProfileDto> GetUserProfileDataAsync(long id);
    Task<byte[]> ExportUserData();
    Task<string> LoginUserAsync(LoginUserDto user);
    Task ChangePasswordAsync(ChangePasswordDto user);
    public void LogoutUser();
    Task RegisterUserAsync(RegisterUserDto user);
    Task UpdateUserAsync(UpdateUserDto user);
    Task UpdateProfilePhotoAsync(ProfilePhotoUpdateDto user);
    Task DeleteUserAsync(long id);
    Task PermanentDeleteUserAsync(long id);
    Task<(byte[], string)> GenerateLibraryCardPdfAsync(long userId);
}
