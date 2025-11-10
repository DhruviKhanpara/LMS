using LMS.Application.Contracts.DTOs.Penalty;
using LMS.Application.Contracts.DTOs.Transection;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.Helpers;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace LMS.Presentation.Controllers;

[Authorize]
public class PenaltyController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;
    private readonly HttpContext _httpContext;

    public PenaltyController(IServiceManager serviceManager, IToastNotification toast, IHttpContextAccessor httpContextAccessor)
    {
        _serviceManager = serviceManager;
        _toast = toast;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    [HttpGet]
    [Route(("penalty/get-penalty"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetPenalty(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null, long? user = null, long? penaltyType = null)
    {
        var paginatedPenalty = await _serviceManager.PenaltyService.GetAllPenaltyAsync<GetPenaltyDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData, userId: user, penaltyTypeId: penaltyType);

        paginatedPenalty.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "Users", await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User, (long)RoleListEnum.Librarian }) },
            { "PenaltyType", Enum.GetValues(typeof(PenaltyTypeEnum))
                .Cast<PenaltyTypeEnum>()
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList()
            }
        };

        if (paginatedPenalty.Data is null || !paginatedPenalty.Data.Any())
            _toast.AddAlertToastMessage("No penalty data are available");

        return View(paginatedPenalty);
    }

    [HttpGet]
    [Route(("penalty/get-user-penalty"))]
    [AuthorizeRoles(RoleListEnum.User, RoleListEnum.Librarian)]
    public async Task<IActionResult> GetUserPenalty(int pageNumber = 1, int pageSize = 10)
    {
        var paginatedPenalty = await _serviceManager.PenaltyService.GetUserPenaltyAsync(pageSize: pageSize, pageNumber: pageNumber);

        if (paginatedPenalty.Data is null || !paginatedPenalty.Data.Any())
            _toast.AddAlertToastMessage("No un-paid penalty data are available");

        if (paginatedPenalty.Data1 is null || !paginatedPenalty.Data1.Any())
            _toast.AddAlertToastMessage("No past penalty data are available");

        return View(paginatedPenalty);
    }

    [HttpGet]
    [Route("penalty/penalty-details")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetPenaltyDetailById(long id)
    {
        var penalty = await _serviceManager.PenaltyService.GetPenaltyByIdAsync<GetPenaltyDto>(id: id);
        return View(penalty);
    }

    [HttpGet]
    [Route("penalty/export-penalty")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportPenalty()
    {
        byte[] excelStream = await _serviceManager.PenaltyService.ExportPenaltyData();
        string fileName = "Penalty-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("penalty/add-penalty")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddPenalty()
    {
        AddPenaltyDto penalty = new AddPenaltyDto();

        if (_httpContext.GetUserRole() == nameof(RoleListEnum.Admin))
            penalty.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User, (long)RoleListEnum.Librarian });
        else if (_httpContext.GetUserRole() == nameof(RoleListEnum.Librarian))
            penalty.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });

        penalty.FineStatusList = Enum.GetValues(typeof(FineStatusEnum))
            .Cast<FineStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        penalty.PenaltyTypeList = Enum.GetValues(typeof(PenaltyTypeEnum))
            .Cast<PenaltyTypeEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        ViewData["UseLayout"] = true;
        return View(penalty);
    }

    [HttpPost]
    [Route("penalty/add-penalty")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddPenalty(AddPenaltyDto penalty)
    {
        if (!ModelState.IsValid)
        {
            if (_httpContext.GetUserRole() == nameof(RoleListEnum.Admin))
                penalty.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User, (long)RoleListEnum.Librarian });
            else if (_httpContext.GetUserRole() == nameof(RoleListEnum.Librarian))
                penalty.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });

            penalty.FineStatusList = Enum.GetValues(typeof(FineStatusEnum))
                .Cast<FineStatusEnum>()
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            penalty.PenaltyTypeList = Enum.GetValues(typeof(PenaltyTypeEnum))
                .Cast<PenaltyTypeEnum>()
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            ViewData["UseLayout"] = false;
            return PartialView("AddPenalty", penalty);
        }

        await _serviceManager.PenaltyService.AddPenaltyAsync(penalty: penalty);

        TempData["SuccessToast"] = "Penalty added successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("penalty/update-penalty")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdatePenalty(long id)
    {
        var penalty = await _serviceManager.PenaltyService.GetPenaltyByIdAsync<UpdatePenaltyDto>(id: id);

        penalty.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: penalty.UserId);

        penalty.FineStatusList = Enum.GetValues(typeof(FineStatusEnum))
            .Cast<FineStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        penalty.PenaltyTypeList = Enum.GetValues(typeof(PenaltyTypeEnum))
            .Cast<PenaltyTypeEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        penalty.Transections = (await _serviceManager.TransectionService.GetAllTransectionAsync<GetTransectionDto>(isActive: true, userId: penalty.UserId)).Data;

        var transectionStatusList = new List<TransectionStatusEnum>
        {
            TransectionStatusEnum.Returned,
            TransectionStatusEnum.Renewed
        };

        if (penalty.Transections.Any(x => x.StatusLabel == nameof(TransectionStatusEnum.ClaimedLost)))
            transectionStatusList.Add(TransectionStatusEnum.ClaimedLost);

        penalty.TransectionStatus = Enum.GetValues(typeof(TransectionStatusEnum))
            .Cast<TransectionStatusEnum>()
            .Where(x => transectionStatusList.Contains(x))
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        ViewData["UseLayout"] = true;
        return View(penalty);
    }

    [HttpPost]
    [Route("penalty/update-penalty")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdatePenalty(UpdatePenaltyDto penalty)
    {
        if (penalty.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            penalty.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: penalty.UserId);

            penalty.FineStatusList = Enum.GetValues(typeof(FineStatusEnum))
                .Cast<FineStatusEnum>()
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            penalty.PenaltyTypeList = Enum.GetValues(typeof(PenaltyTypeEnum))
                .Cast<PenaltyTypeEnum>()
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            penalty.Transections = (await _serviceManager.TransectionService.GetAllTransectionAsync<GetTransectionDto>(isActive: true, userId: penalty.UserId)).Data;

            var transectionStatusList = new List<TransectionStatusEnum>
            {
                TransectionStatusEnum.Returned,
                TransectionStatusEnum.Renewed
            };

            if (penalty.Transections.Any(x => x.StatusLabel == nameof(TransectionStatusEnum.ClaimedLost)))
                transectionStatusList.Add(TransectionStatusEnum.ClaimedLost);

            penalty.TransectionStatus = Enum.GetValues(typeof(TransectionStatusEnum))
                .Cast<TransectionStatusEnum>()
                .Where(x => transectionStatusList.Contains(x))
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            ViewData["UseLayout"] = false;
            return PartialView("UpdatePenalty", penalty);
        }

        await _serviceManager.PenaltyService.UpdatePenaltyAsync(penalty: penalty);

        TempData["SuccessToast"] = "Penalty updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }


    [HttpGet]
    [Route("penalty/remove-penalty")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> RemovePenalty(long id)
    {
        await _serviceManager.PenaltyService.RemovePenaltyAsync(id: id);
        _toast.AddSuccessToastMessage("Penalty removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetPenalty", "Penalty");
        }
    }

    [HttpGet]
    [Route("penalty/delete-penalty")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeletePenalty(long id)
    {
        await _serviceManager.PenaltyService.PermanentDeletePenaltyAsync(id: id);

        _toast.AddSuccessToastMessage("Penalty deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetPenalty", "Penalty");
        }
    }
}
