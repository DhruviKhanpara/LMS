using LMS.Application.Contracts.DTOs.Membership;
using LMS.Application.Contracts.DTOs.UserMembershipMapping;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.Helpers;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using LMS.Presentation.Views.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace LMS.Presentation.Controllers;

[Authorize]
public class UserMembershipMappingController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;
    private readonly HttpContext _httpContext;

    public UserMembershipMappingController(IServiceManager serviceManager, IToastNotification toast, IHttpContextAccessor httpContextAccessor)
    {
        _serviceManager = serviceManager;
        _toast = toast;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    [HttpPost]
    [Route("userMembershipMapping/current-plan-of-login-user")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> GetAvailableLoginUserPlan([FromBody] GetMembershipDto? selectedMembership = null)
    {
        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);

        var loginUserMembership = await _serviceManager.UserMembershipMappingService.GetUserMembershipAsync<GetUserMembershipDto>(isActive: true, userId: authUserId);
        long.TryParse((await _serviceManager.ConfigsService.GetConfigsByKeyName(new List<string> { "MaxActiveMembership" })).FirstOrDefault()?.KeyValue ?? "0", out long maxLimit);

        var model = new PlanSelectionModelBodyViewModel()
        {
            MaxAvailablePlanLimit = maxLimit,
            SelectedPlan = selectedMembership,
            AvailablePlans = loginUserMembership.Data
        };

        return View(model);
    }

    [HttpGet]
    [Route("userMembershipMapping/get-all-user-memberships")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetUserMemberships(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null, long? user = null)
    {
        var paginatedUserMemberships = await _serviceManager.UserMembershipMappingService.GetUserMembershipAsync<GetUserMembersipListDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData, userId: user);

        paginatedUserMemberships.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "Users", await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User }) }
        };

        if (paginatedUserMemberships.Data is null || !paginatedUserMemberships.Data.Any())
            _toast.AddAlertToastMessage("No User membership data are available");

        return View(paginatedUserMemberships);
    }

    [HttpGet]
    [Route("userMembershipMapping/get-login-user-memberships")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> GetLoginUserMembership()
    {
        await _serviceManager.UserMembershipMappingService.RemoveExpiredUserMembershipAsync(forLogin: true);
        var userMemberships = await _serviceManager.UserMembershipMappingService.GetLoginUserMembershipAsync();

        if (userMemberships is null || !userMemberships.Any())
            _toast.AddAlertToastMessage("No active User membership data are available");

        return View(userMemberships);
    }

    [HttpGet]
    [Route(("userMembershipMapping/userMembershipMapping-detail"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetUserMembershipDetailById(long id)
    {
        var userMembership = await _serviceManager.UserMembershipMappingService.GetUserMembershipByIdAsync<GetUserMembersipListDto>(id: id);
        return View(userMembership);
    }

    [HttpGet]
    [Route("userMembershipMapping/export-userMembershipMapping")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportUserMemberships()
    {
        byte[] excelStream = await _serviceManager.UserMembershipMappingService.ExportUserMembershipData();
        string fileName = "UserMemberships-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("userMembershipMapping/purchase-new-plan")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> AddNewPlan(long membershipId)
    {
        await _serviceManager.UserMembershipMappingService.AddOrUpgradeUserMembershipAsync(membershipId: membershipId);
        _toast.AddSuccessToastMessage("Purchased successfully");

        return RedirectToAction("Plan", "Membership");
    }

    [HttpGet]
    [Route("userMembershipMapping/upgrade-and-purchase-new-plan")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> UpgradeLatestPlan(long membershipId)
    {
        await _serviceManager.UserMembershipMappingService.AddOrUpgradeUserMembershipAsync(membershipId: membershipId, IsUpgradePlan: true);
        _toast.AddSuccessToastMessage("Upgrade and Purchased successfully");

        return RedirectToAction("Plan", "Membership");
    }

    [HttpGet]
    [Route("userMembershipMapping/add-new-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddUserMembership()
    {
        AddUserMembershipDto userMembership = new AddUserMembershipDto();

        userMembership.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });
        userMembership.Memberships = (await _serviceManager.MembershipService.GetAllMembershipAsync<GetMembershipDto>(isActive: true)).Data;
        userMembership.AddPlanOptions = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "Add new", Value = "false" },
            new SelectListItem() { Text = "Upgrade exsiting", Value = "true" }
        };
        userMembership.DisableAddForUserCondition = await _serviceManager.UserMembershipMappingService.GetUserEligibilityToAdd();

        return View(userMembership);
    }

    [HttpPost]
    [Route("userMembershipMapping/add-new-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddUserMembership(AddUserMembershipDto userMembership)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            userMembership.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });
            userMembership.Memberships = (await _serviceManager.MembershipService.GetAllMembershipAsync<GetMembershipDto>(isActive: true)).Data;
            userMembership.AddPlanOptions = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Add new", Value = "false" },
                new SelectListItem() { Text = "Upgrade exsiting", Value = "true" }
            };
            userMembership.DisableAddForUserCondition = await _serviceManager.UserMembershipMappingService.GetUserEligibilityToAdd();

            return PartialView("AddUserMembership", userMembership);
        }

        await _serviceManager.UserMembershipMappingService.AddOrUpgradeUserMembershipAsync(membershipId: userMembership.MembershipId, IsUpgradePlan: userMembership.IsUpgradePlan ?? false, userId: userMembership.UserId);

        TempData["SuccessToast"] = "User Membership added successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("userMembershipMapping/update-user-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateUserMembership(long id)
    {
        var userMembership = await _serviceManager.UserMembershipMappingService.GetUserMembershipByIdAsync<UpdateUserMembershipDto>(id: id);

        userMembership.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: userMembership.UserId);
        userMembership.Memberships = new List<GetMembershipDto>() { await _serviceManager.MembershipService.GetMembershipByIdAsync<GetMembershipDto>(id: userMembership.MembershipId) };

        return View(userMembership);
    }

    [HttpPost]
    [Route("userMembershipMapping/update-user-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateUserMembership(UpdateUserMembershipDto userMembership)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            userMembership.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: userMembership.UserId);
            userMembership.Memberships = new List<GetMembershipDto>() { await _serviceManager.MembershipService.GetMembershipByIdAsync<GetMembershipDto>(id: userMembership.MembershipId) };

            return PartialView("UpdateUserMembership", userMembership);
        }

        await _serviceManager.UserMembershipMappingService.UpdateUserMembershipAsync(userMembership);

        TempData["SuccessToast"] = "User Membership updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("userMembershipMapping/remove-userMembershipMapping")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> DeleteUserMembership(long id)
    {
        await _serviceManager.UserMembershipMappingService.DeleteUserMembershipAsync(id: id);

        _toast.AddSuccessToastMessage("User Membership removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetUserMemberships", "UserMembershipMapping");
        }
    }

    [HttpGet]
    [Route("userMembershipMapping/delete-userMembershipMapping")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteUserMembership(long id)
    {
        await _serviceManager.UserMembershipMappingService.PermanentDeleteUserMembershipAsync(id: id);

        _toast.AddSuccessToastMessage("User Membership deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetUserMemberships", "UserMembershipMapping");
        }
    }
}