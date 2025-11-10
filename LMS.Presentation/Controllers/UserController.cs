using LMS.Application.Contracts.DTOs.User;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.Helpers;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace LMS.Presentation.Controllers;

public class UserController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;
    private readonly HttpContext _httpContext;

    public UserController(IServiceManager serviceManager, IToastNotification toast, IHttpContextAccessor httpContextAccessor)
    {
        _serviceManager = serviceManager;
        _toast = toast;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    [HttpGet]
    [Route("user/get-user")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null)
    {
        var paginatedUsers = await _serviceManager.UserService.GetAllUserAsync<GetUserDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData);

        if (paginatedUsers.Data is null || !paginatedUsers.Data.Any())
            _toast.AddAlertToastMessage("No User data are available");

        return View(paginatedUsers);
    }

    [HttpGet]
    [Route(("user/user-detail"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetUserDetailById(long id)
    {
        var user = await _serviceManager.UserService.GetUserByIdAsync<GetUserDetailDto>(id: id);
        return View(user);
    }

    [HttpGet]
    [Route(("user/user-profile"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin, RoleListEnum.User)]
    public async Task<IActionResult> GetUserProfile()
    {
        long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var user = await _serviceManager.UserService.GetUserProfileDataAsync(id: authUserId);
        return View(user);
    }

    [HttpGet]
    [Route("user/export-user")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportUsers()
    {
        byte[] excelStream = await _serviceManager.UserService.ExportUserData();
        string fileName = "Users-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("user/login-user")]
    public IActionResult Login()
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        LoginUserDto user = new LoginUserDto();
        return View(user);
    }

    [HttpPost]
    [Route("user/login-user")]
    public async Task<IActionResult> Login(LoginUserDto user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var loginUserRole = await _serviceManager.UserService.LoginUserAsync(user: user);

        _toast.AddSuccessToastMessage("Login successfully");

        if (loginUserRole == nameof(RoleListEnum.User))
        {
            return RedirectToAction("UserDashboard", "Home");
        }
        else if (loginUserRole == nameof(RoleListEnum.Librarian))
        {
            return RedirectToAction("LibrarianDashboard", "Home");
        }
        else if (loginUserRole == nameof(RoleListEnum.Admin))
        {
            return RedirectToAction("AdminDashboard", "Home");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    [Route("user/register-user")]
    public IActionResult Registration()
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [Route("user/register-user")]
    public async Task<IActionResult> Registration(RegisterUserDto user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        user.RoleId = (long)RoleListEnum.User;
        await _serviceManager.UserService.RegisterUserAsync(user: user);

        _toast.AddSuccessToastMessage("Register successfully");
        return RedirectToAction("Login", "User");
    }

    [Route("user/logout-user")]
    [Authorize]
    public IActionResult Logout()
    {
        if (User.Identity!.IsAuthenticated)
        {
            _serviceManager.UserService.LogoutUser();
            _toast.AddSuccessToastMessage("Logout successfully");
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("user/add-account")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public IActionResult AddUser()
    {
        var user = new RegisterUserDto();
        ViewData["UseLayout"] = true;

        if (_httpContext.GetUserRole() == nameof(RoleListEnum.Admin))
        {
            user.RoleList = Enum.GetValues(typeof(RoleListEnum))
            .Cast<RoleListEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();
        }
        else if (_httpContext.GetUserRole() == nameof(RoleListEnum.Librarian))
        {
            user.RoleList = Enum.GetValues(typeof(RoleListEnum))
            .Cast<RoleListEnum>()
            .Where(x => x.Equals(RoleListEnum.User))
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

            user.RoleId = (long)RoleListEnum.User;
        }

        return View(user);
    }

    [HttpPost]
    [Route("user/add-account")]
    public async Task<IActionResult> AddUser(RegisterUserDto user)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            if (_httpContext.GetUserRole() == nameof(RoleListEnum.Admin))
            {
                user.RoleList = Enum.GetValues(typeof(RoleListEnum))
                .Cast<RoleListEnum>()
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();
            }
            else if (_httpContext.GetUserRole() == nameof(RoleListEnum.Librarian))
            {
                user.RoleList = Enum.GetValues(typeof(RoleListEnum))
                .Cast<RoleListEnum>()
                .Where(x => x.Equals(RoleListEnum.User))
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

                user.RoleId = (long)RoleListEnum.User;
            }

            return PartialView("AddUser", user);
        }

        await _serviceManager.UserService.RegisterUserAsync(user: user);

        _toast.AddSuccessToastMessage("Account created successfully");
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("user/update-account")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin, RoleListEnum.User)]
    public async Task<IActionResult> UpdateUser(long id)
    {
        var user = await _serviceManager.UserService.GetUserByIdAsync<UpdateUserDto>(id: id);
        ViewData["UseLayout"] = true;

        user.RoleList = Enum.GetValues(typeof(RoleListEnum))
            .Cast<RoleListEnum>()
            .Where(x => (long)x == user.RoleId)
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        return View(user);
    }

    [HttpPost]
    [Route("user/update-account")]
    public async Task<IActionResult> UpdateUser(UpdateUserDto user)
    {
        if (user.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            user.RoleList = Enum.GetValues(typeof(RoleListEnum))
            .Cast<RoleListEnum>()
            .Where(x => (long)x == user.RoleId)
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

            return PartialView("UpdateUser", user);
        }

        await _serviceManager.UserService.UpdateUserAsync(user: user);

        TempData["SuccessToast"] = "Account updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("user/change-user-password")]
    public IActionResult ChangePassword()
    {
        ChangePasswordDto user = new ChangePasswordDto();
        return View(user);
    }

    [HttpPost]
    [Route("user/change-user-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        await _serviceManager.UserService.ChangePasswordAsync(user: user);

        _toast.AddSuccessToastMessage("Password update successfully");

        if (User.Identity!.IsAuthenticated)
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        else
            return Json(new { success = true, visit = Url.Action("Login", "User") });
    }

    [HttpGet]
    [Route("user/auth-change-user-password")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public IActionResult AuthChangePassword()
    {
        ChangePasswordDto user = new ChangePasswordDto();
        return View(user);
    }

    [HttpPost]
    [Route("user/auth-change-user-password")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AuthChangePassword(ChangePasswordDto user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        await _serviceManager.UserService.ChangePasswordAsync(user: user);

        TempData["SuccessToast"] =  "Password update successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("user/change-user-profile-photo")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin, RoleListEnum.User)]
    public async Task<IActionResult> ChangeProfileAsync()
    {
        long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var user = await _serviceManager.UserService.GetUserByIdAsync<ProfilePhotoUpdateDto>(id: authUserId);
        return View(user);
    }

    [HttpPost]
    [Route("user/change-user-profile-photo")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin, RoleListEnum.User)]
    public async Task<IActionResult> ChangeProfile(ProfilePhotoUpdateDto user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        await _serviceManager.UserService.UpdateProfilePhotoAsync(user: user);

        TempData["SuccessToast"] = "Profile photo update successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("user/remove-user")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> DeleteUser(long id)
    {
        await _serviceManager.UserService.DeleteUserAsync(id: id);
        await _serviceManager.ReservationService.RemoveUserReservationAsync(userId: id);
        await _serviceManager.UserMembershipMappingService.DeleteAllUserMembershipAsync(userId: id);

        _toast.AddSuccessToastMessage("User removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetUsers", "User");
        }
    }

    [HttpGet]
    [Route("user/generate-user-library-card")]
    [AuthorizeRoles(RoleListEnum.User, RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> DownloadLibraryCard(long userId)
    {
        var (pdfContent, fileName) = await _serviceManager.UserService.GenerateLibraryCardPdfAsync(userId: userId);

        return File(pdfContent, "application/pdf", fileName);
    }

    [HttpGet]
    [Route("user/delete-user")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteUser(long id)
    {
        await _serviceManager.UserService.PermanentDeleteUserAsync(id: id);

        _toast.AddSuccessToastMessage("User deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetUser", "User");
        }
    }
}
