using LMS.Application.Contracts.DTOs.Membership;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace LMS.Presentation.Controllers;

public class MembershipController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;

    public MembershipController(IServiceManager serviceManager, IToastNotification toast)
    {
        _serviceManager = serviceManager;
        _toast = toast;
    }

    [HttpGet]
    [Route("membership/get-all-plan")]
    public async Task<IActionResult> Plan()
    {
        var memberships = await _serviceManager.MembershipService.GetAllMembershipAsync<GetMembershipDto>(isActive: true);

        if (memberships.Data is null || !memberships.Data.Any())
            _toast.AddAlertToastMessage("No membership plan are available");

        return View(memberships.Data);
    }

    [HttpGet]
    [Route("membership/get-all-memberships")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetMemberships(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null)
    {
        var memberships = await _serviceManager.MembershipService.GetAllMembershipAsync<GetMembershipDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData);

        if (memberships.Data is null || !memberships.Data.Any())
            _toast.AddAlertToastMessage("No membership plan are available");

        return View(memberships);
    }

    [HttpGet]
    [Route(("membership/membership-detail"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetMembershipDetailById(long id)
    {
        var membership = await _serviceManager.MembershipService.GetMembershipByIdAsync<GetMembershipDto>(id: id);
        return View(membership);
    }

    [HttpGet]
    [Route("membership/export-membership")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportMembership()
    {
        byte[] excelStream = await _serviceManager.MembershipService.ExportMembershipData();
        string fileName = "Memberships-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("membership/create-membership-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public IActionResult CreateMembership()
    {
        AddMembershipDto membership = new AddMembershipDto();

        ViewData["UseLayout"] = true;
        return View(membership);
    }

    [HttpPost]
    [Route("membership/create-membership-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> CreateMembership(AddMembershipDto membership)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;
            return PartialView("CreateMemebrship", membership);
        }

        await _serviceManager.MembershipService.AddMembershipAsync(membership: membership);

        TempData["SuccessToast"] = "Membership plan created successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("membership/update-membership-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateMembership(long id)
    {
        var membership = await _serviceManager.MembershipService.GetMembershipByIdAsync<UpdateMembershipDto>(id: id);

        ViewData["UseLayout"] = true;
        return View(membership);
    }

    [HttpPost]
    [Route("membership/update-membership-plan")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateMembership(UpdateMembershipDto membership)
    {
        if (membership.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;
            return PartialView("UpdateMembership", membership);
        }

        await _serviceManager.MembershipService.UpdateMembershipAsync(membership: membership);

        TempData["SuccessToast"] = "Membership plan updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("membership/remove-membership-plan")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> DeleteMembership(long id)
    {
        await _serviceManager.MembershipService.DeleteMembershipAsync(id: id);

        _toast.AddSuccessToastMessage("Membership plan removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetMemberships", "Membership");
        }
    }

    [HttpGet]
    [Route("membership/delete-membership-plan")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteMembership(long id)
    {
        await _serviceManager.MembershipService.PermanentDeleteMembershipAsync(id: id);

        _toast.AddSuccessToastMessage("Membership deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetMemberships", "Membership");
        }
    }
}
