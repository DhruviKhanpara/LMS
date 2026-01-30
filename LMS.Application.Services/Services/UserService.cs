using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.DTOs;
using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.DTOs.User;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;
using LMS.Application.Contracts.Interfaces.ExternalServices;
using LMS.Application.Contracts.Interfaces.Repositories;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Application.Services.Constants;
using LMS.Common.ErrorHandling.CustomException;
using LMS.Common.Helpers;
using LMS.Common.Models;
using LMS.Common.Security;
using LMS.Core.Entities;
using LMS.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections;

namespace LMS.Application.Services.Services;

internal class UserService : IUserService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IExternalServiceManager _externalServiceManager;
    private readonly HttpContext? _httpContext;
    private readonly TokenService _tokenService;
    private readonly IValidationService _validationService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserService(IRepositoryManager repositoryManager, IExternalServiceManager externalServiceManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IValidationService validationService, IConfiguration configuration, TokenService tokenService)
    {
        _repositoryManager = repositoryManager;
        _externalServiceManager = externalServiceManager;
        _httpContext = httpContextAccessor.HttpContext;
        _validationService = validationService;
        _configuration = configuration;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<PaginatedResponseDto<T>> GetAllUserAsync<T>(int? pageSize = null, int? pageNumber = null, string? orderBy = null, bool? isActive = null) where T : class
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var userQuery = _repositoryManager.UserRepository
            .GetAllAsync(isActive: _httpContext!.GetUserRole().Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase) ? isActive : true);

        if (_httpContext!.GetUserRole().Equals(RoleListEnum.Librarian.ToString(), StringComparison.InvariantCultureIgnoreCase))
            userQuery = userQuery.Where(x => x.RoleId == (long)RoleListEnum.User);
        else if (_httpContext!.GetUserRole().Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase))
            userQuery = userQuery.Where(x => x.Id == authUserId);

        var totalCount = await userQuery.CountAsync();

        var paginationModel = (pageNumber.HasValue && pageSize.HasValue)
            ? new PaginationModel(pageNumber: (int)pageNumber, pageSize: (int)pageSize, totalCount: totalCount)
            : new PaginationModel(totalCount: totalCount);

        var users = await userQuery
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .Sort<T>(orderBy: orderBy)
            .Paginate(pageSize: paginationModel.PageSize, pageNumber: paginationModel.PageNumber)
            .ToListAsync();

        return new PaginatedResponseDto<T>()
        {
            Pagination = paginationModel,
            Data = users
        };
    }

    public async Task<List<SelectListItem>> GetAllUserSelectionAsync(long[]? userRoleList = null, long? userId = null)
    {
        var userSelection = await _repositoryManager.UserRepository.GetAllAsync(isActive: true)
            .Where(x => (userId == null || x.Id == userId) && (userRoleList == null || userRoleList.Contains(x.RoleId)))
            .ProjectTo<SelectListItem>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return userSelection ?? new List<SelectListItem>();
    }

    public async Task<T> GetUserByIdAsync<T>(long id) where T : class
    {
        var user = await _repositoryManager.UserRepository
            .GetByIdAsync(id)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return user ?? Activator.CreateInstance<T>();
    }

    public async Task<UserProfileDto> GetUserProfileDataAsync(long id)
    {
        var user = await _repositoryManager.UserRepository
            .GetByIdAsync(id)
            .Include(u => u.Penalties)
            .Include(u => u.Transections)
            .Include(u => u.UserMemberships)
            .Include(u => u.Reservations)
            .Include(u => u.Role)
            .FirstOrDefaultAsync();

        if (user == null)
            return new UserProfileDto();

        var userProfile = _mapper.Map<UserProfileDto>(user);

        return userProfile;
    }

    public async Task<byte[]> ExportUserData()
    {
        var users = await GetAllUserAsync<ExportUserDto>();

        return FileService.ExportMultipleSheets(new Dictionary<string, IEnumerable> { { "User", users.Data } });
    }

    public async Task<string> LoginUserAsync(LoginUserDto user)
    {
        _validationService.Validate<LoginUserDto>(user);

        if (!await _repositoryManager.UserRepository.AnyAsync(x => x.Email.ToLower() == user.EmailOrUsername!.ToLower() || x.Username.ToLower() == user.EmailOrUsername.ToLower() && x.IsActive))
            throw new BadRequestException("Invalid Email or Username");

        var existUser = await _repositoryManager.UserRepository
            .FindByCondition(x => x.Email.ToLower() == user.EmailOrUsername!.ToLower() || x.Username.ToLower() == user.EmailOrUsername.ToLower() && x.IsActive)
            .FirstOrDefaultAsync();

        if (!PasswordHasher.VerifyPassword(password: user.Password!, passwordHash: existUser!.PasswordHash, passwordSolt: existUser!.PasswordSolt))
            throw new BadRequestException("Invalid credentials, may your password is wrong");

        var profilePath = existUser!.ProfilePhoto is not null ? "/" + existUser!.ProfilePhoto.Replace("\\", "/") : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s";

        string token = _tokenService.CreateToken(email: existUser!.Email, role: Enum.GetName(typeof(RoleListEnum), existUser!.RoleId) ?? "", id: existUser!.Id, name: existUser!.FirstName, profilePhoto: profilePath);

        _httpContext?.Response.Cookies.Append("AccessToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(double.Parse(_configuration.GetSection("AppSettings:jwtTokenExpirationMinutes").Value!))
        });

        return Enum.GetName(typeof(RoleListEnum), existUser!.RoleId) ?? "";
    }

    public async Task ChangePasswordAsync(ChangePasswordDto user)
    {
        _validationService.Validate<ChangePasswordDto>(user);

        if (!await _repositoryManager.UserRepository.AnyAsync(x => x.Email.ToLower() == user.EmailOrUsername!.ToLower() || x.Username.ToLower() == user.EmailOrUsername.ToLower() && x.IsActive))
            throw new BadRequestException("Invalid Email or Username");

        var existUser = await _repositoryManager.UserRepository
            .FindByCondition(x => x.Email.ToLower() == user.EmailOrUsername!.ToLower() || x.Username.ToLower() == user.EmailOrUsername.ToLower() && x.IsActive)
            .FirstOrDefaultAsync();

        if (existUser == null)
            throw new BadRequestException("User is not Found");

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        bool isLibrarian = authUserRole.Equals(RoleListEnum.Librarian.ToString(), StringComparison.InvariantCultureIgnoreCase);
        bool isUser = authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase);
        bool isAdmin = authUserRole.Equals(RoleListEnum.Admin.ToString(), StringComparison.InvariantCultureIgnoreCase);
        bool hasDifferentRoleOrId = existUser.RoleId != (long)RoleListEnum.User || existUser.Id != authUserId;
        bool isUnauthorizedUser = isUser && existUser.Id != authUserId;

        if (isLogin && (isUnauthorizedUser || (isLibrarian && hasDifferentRoleOrId)))
            throw new BadRequestException("You don't have an permission for update this user password");

        if (user.NewPassword != user.ConfirmPassword)
            throw new BadRequestException("Password and ConfirmPassword are different");

        if (!((isLibrarian && existUser.RoleId == (long)RoleListEnum.User) || isAdmin) && string.IsNullOrWhiteSpace(user.Password))
            throw new BadRequestException("Password is Required");

        if (user.Password != null && !PasswordHasher.VerifyPassword(password: user.Password, passwordHash: existUser.PasswordHash, passwordSolt: existUser!.PasswordSolt))
            throw new BadRequestException("Invalid credentials, may your password is wrong");

        PasswordHasher.CreatePasswordHash(user.NewPassword!, out byte[] passwordHash, out byte[] passwordSolt);
        existUser.PasswordHash = passwordHash;
        existUser.PasswordSolt = passwordSolt;

        _repositoryManager.UserRepository.Update(entity: existUser);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public void LogoutUser()
    {
        _httpContext?.Response.Cookies.Delete("AccessToken");
    }

    public async Task RegisterUserAsync(RegisterUserDto user)
    {
        _validationService.Validate<RegisterUserDto>(user);

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        if (!Enum.IsDefined(typeof(RoleListEnum), user.RoleId) || ((!isLogin || _httpContext!.GetUserRole().ToLower() == RoleListEnum.Librarian.ToString().ToLower()) && user.RoleId != (long)RoleListEnum.User))
            throw new BadRequestException("You haven't permission to register for this role");

        if (user.Password != user.ConfirmPassword)
            throw new BadRequestException("Password and ConfirmPassword are different");

        if (await _repositoryManager.UserRepository.IsUserExistenceAsync(username: user.Username!, email: user.Email!, roleId: user.RoleId))
            throw new BadRequestException("This Username and Email is already registered");

        PasswordHasher.CreatePasswordHash(user.Password!, out byte[] passwordHash, out byte[] passwordSolt);
        var mappedUser = _mapper.Map<User>(user);
        mappedUser.PasswordHash = passwordHash;
        mappedUser.PasswordSolt = passwordSolt;

        if (user.RoleId == (long)RoleListEnum.User)
            mappedUser.LibraryCardNumber = await GenerateLibraryCardNoForUser();

        if (_httpContext!.GetUserRole().ToLower() == RoleListEnum.User.ToString().ToLower())
            mappedUser.JoiningDate = DateTimeOffset.UtcNow;

        if (user.ProfilePhoto is not null)
        {
            mappedUser.ProfilePhoto = await AddUserProfile(profilePhoto: user.ProfilePhoto);
        }

        await _repositoryManager.UserRepository.AddAsync(entity: mappedUser);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(UpdateUserDto user)
    {
        _validationService.Validate<UpdateUserDto>(user);

        var existUser = await _repositoryManager.UserRepository.GetByIdAsync(id: user.Id).FirstOrDefaultAsync();

        if (existUser == null)
            throw new BadRequestException("This User is not available");

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || (authUserRole.Equals(RoleListEnum.Librarian.ToString(), StringComparison.InvariantCultureIgnoreCase) && (existUser.RoleId != (long)RoleListEnum.User || existUser.Id != authUserId))
            || (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) && existUser.Id != authUserId))
            throw new BadRequestException("You don't have an permission for update this user detail");

        if (await _repositoryManager.UserRepository.IsUserExistenceAsync(username: user.Username!, email: user.Email!, roleId: existUser.RoleId, userId: existUser.Id))
            throw new BadRequestException("This Username and Email is already registered");

        var mappedUser = _mapper.Map(user, existUser);

        if (mappedUser.ProfilePhoto is not null && user.IsDeletedProfile)
        {
            await RemoveUserProfile(profilePhotoPath: mappedUser.ProfilePhoto);
            mappedUser.ProfilePhoto = null;
        }

        if (user.ProfilePhoto is not null)
        {
            mappedUser.ProfilePhoto = await AddUserProfile(profilePhoto: user.ProfilePhoto);
        }

        _repositoryManager.UserRepository.Update(entity: mappedUser);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task UpdateProfilePhotoAsync(ProfilePhotoUpdateDto user)
    {
        _validationService.Validate<ProfilePhotoUpdateDto>(user);

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin)
            throw new BadRequestException("You don't have an permission for update this user profile");

        var existUser = await _repositoryManager.UserRepository.GetByIdAsync(id: authUserId).FirstOrDefaultAsync();

        if (existUser == null)
            throw new BadRequestException("User is not Found");

        if (existUser.ProfilePhoto is not null && user.IsDeletedProfile)
        {
            await RemoveUserProfile(profilePhotoPath: existUser.ProfilePhoto);
            existUser.ProfilePhoto = null;
        }

        if (user.ProfilePhoto is not null)
        {
            existUser.ProfilePhoto = await AddUserProfile(profilePhoto: user.ProfilePhoto);
        }

        _repositoryManager.UserRepository.Update(entity: existUser);
        await _repositoryManager.UnitOfWork.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(long id)
    {
        var existUser = await _repositoryManager.UserRepository.GetByIdAsync(id: id).FirstOrDefaultAsync();

        if (existUser == null)
            throw new BadRequestException("This user is already not exist");

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || (authUserRole.Equals(RoleListEnum.Librarian.ToString(), StringComparison.InvariantCultureIgnoreCase) && (existUser.RoleId != (long)RoleListEnum.User || existUser.Id != authUserId))
            || (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) && existUser.Id != authUserId))
            throw new BadRequestException("You are not authorize to delete this account");

        if (await _repositoryManager.TransectionRepository
            .AnyAsync(x => x.IsActive && x.UserId == id && !StatusGroups.Transaction.Finalized.Contains(x.StatusId)))
            throw new BadRequestException("Have occupied resources");

        if (await _repositoryManager.PenaltyRepository
            .AnyAsync(x => x.IsActive && x.UserId == id && x.StatusId != (long)FineStatusEnum.UnPaid))
            throw new BadRequestException("Have unpaid penalty");

        if (existUser.RoleId == (long)RoleListEnum.Admin
            && (await _repositoryManager.UserRepository
            .FindByCondition(x => x.IsActive && x.RoleId == (long)RoleListEnum.Admin)
            .CountAsync() == 1))
            throw new BadRequestException("This is the only active Admin, you cannot delete this account");

        existUser.IsActive = false;

        if (existUser.ProfilePhoto is not null)
            await RemoveUserProfile(profilePhotoPath: existUser.ProfilePhoto);

        _repositoryManager.UserRepository.Update(entity: existUser);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task PermanentDeleteUserAsync(long id)
    {
        var existUser = await _repositoryManager.UserRepository.GetAllAsync(isActive: false).FirstOrDefaultAsync(x => x.Id == id);

        if (existUser == null)
            throw new BadRequestException("This user is already not exist");

        _repositoryManager.UserRepository.Remove(entity: existUser);
        _repositoryManager.UnitOfWork.SaveChanges();
    }

    public async Task<(byte[], string)> GenerateLibraryCardPdfAsync(long userId)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (!isLogin || (authUserRole.Equals(RoleListEnum.User.ToString(), StringComparison.InvariantCultureIgnoreCase) && userId != authUserId))
            throw new BadRequestException("You are not authorize to download library card for this account");

        var user = await _repositoryManager.UserRepository.GetByIdAsync(id: userId).FirstOrDefaultAsync();

        if (user == null)
            throw new BadRequestException($"Unable to generate library card pdf, user not found");

        var userName = $"{user.FirstName} {user.MiddleName ?? ""} {user.LastName ?? ""}".Trim();
        string base64Barcode = Convert.ToBase64String(_externalServiceManager.BarcodeGenerator.GenerateBarcode(user.LibraryCardNumber ?? ""));

        var replacements = new Dictionary<string, string>
        {
            { "{{ProfilePhoto}}", !string.IsNullOrWhiteSpace(user.ProfilePhoto) ? new Uri(FileService.GetFullFilePath(user.ProfilePhoto)).AbsoluteUri : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSG2h3dtkFclxksGm2bXE8R53sUemVyVGmJTg&s" },
            { "{{UserName}}", userName },
            { "{{LibraryCardNo}}", user.LibraryCardNumber ?? "" },
            { "{{JoiningDate}}", user.JoiningDate.ToString("yyyy-MM-dd") },
            { "{{PhoneNumber}}", user.MobileNo },
            { "{{Barcode}}", $"data:image/png;base64,{base64Barcode}" },
        };

        var pdfContent = _externalServiceManager.PdfGenerator.GenerateLibraryCardPdf("LibraryCardDesign.html", replacements);
        var fileName = $"LibraryCard-{userName}.pdf";

        return (pdfContent, fileName);
    }

    #region Private section
    private async Task<string> AddUserProfile(IFormFile profilePhoto)
    {
        var profilePhotoStorageInfo = await _repositoryManager.ConfigRepository
            .GetByKeyNameListAsync(new List<string>() { "ProfilePhotoDirectoryPath", "ProfilePhotoArchiveDirectoryPath", "ImageFileExtentions" })
            .ProjectTo<GetConfigsValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var sourceFileDirectory = profilePhotoStorageInfo
            .FirstOrDefault(x => x.KeyName.Equals("ProfilePhotoDirectoryPath", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? throw new ArgumentException("Source file directory path not found!");

        var archiveDirectory = profilePhotoStorageInfo
            .FirstOrDefault(x => x.KeyName.Equals("ProfilePhotoArchiveDirectoryPath", StringComparison.InvariantCultureIgnoreCase))?
            .KeyValue ?? throw new ArgumentException("Archive file directory path not found!");

        var imageFileExtentions = profilePhotoStorageInfo
            .FirstOrDefault(x => x.KeyName.Equals("ImageFileExtentions", StringComparison.InvariantCultureIgnoreCase))?.KeyValue
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(ext => ext.Trim())
            .ToArray() ?? throw new ArgumentException("Valid file extension list not found!");

        return await FileService.FileUpload(file: profilePhoto, sourceFileDirectory: sourceFileDirectory, archiveDirectory: archiveDirectory, allowedExtensions: imageFileExtentions);
    }

    private async Task RemoveUserProfile(string profilePhotoPath)
    {
        var archiveDirectory = (await _repositoryManager.ConfigRepository
                .GetByKeyNameAsync("ProfilePhotoArchiveDirectoryPath")
                .FirstOrDefaultAsync())?
                .KeyValue ?? throw new ArgumentException("Archive file directory path not found!");

        await FileService.MoveFileToArchive(sourceFile: profilePhotoPath, archiveDirectory: archiveDirectory);
    }

    private async Task<string> GenerateLibraryCardNoForUser()
    {
        var existingCardNumbers = await _repositoryManager.UserRepository
        .FindByCondition(x => x.LibraryCardNumber != null && x.IsActive)
        .Select(x => x.LibraryCardNumber)
        .ToListAsync();

        var lastNumber = existingCardNumbers
            .OrderByDescending(x => int.TryParse(x.Substring(7), out int num) ? num : 0)
            .FirstOrDefault() ?? $"USR{DateTimeOffset.Now.Year}";

        string newNumber = GenerateUniqueNumber.GenerateLibraryCardNumber(lastNumber);

        return newNumber ?? throw new BadRequestException("Library card number not generate");
    }
    #endregion
}
