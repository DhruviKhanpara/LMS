using LMS.Application.Contracts.DTOs.Configs;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace LMS.Presentation.Controllers;

[Authorize]
public class ConfigsController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;

    public ConfigsController(IServiceManager serviceManager, IToastNotification toast)
    {
        _serviceManager = serviceManager;
        _toast = toast;
    }

    [HttpGet]
    [Route("configs/get-configs")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetConfigs(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null)
    {
        var paginatedConfigs = await _serviceManager.ConfigsService.GetAllConfigsAsync<GetConfigsDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData);

        if (paginatedConfigs.Data is null || !paginatedConfigs.Data.Any())
            _toast.AddAlertToastMessage("No Configs data are available");

        return View(paginatedConfigs);
    }

    [HttpGet]
    [Route(("configs/configs-detail"))]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetConfigsDetailById(long id)
    {
        var config = await _serviceManager.ConfigsService.GetConfigsByIdAsync<GetConfigsDto>(id: id);
        return View(config);
    }

    [HttpGet]
    [Route("rules/get-instructions")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> GetIntructions()
    {
        var configs = await _serviceManager.ConfigsService.GetAllConfigValues();

        if (configs is null)
            _toast.AddAlertToastMessage("No Configs data are available");

        return View(configs);
    }

    [HttpGet]
    [Route("configs/export-configs")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportConfigs()
    {
        byte[] excelStream = await _serviceManager.ConfigsService.ExportConfigData();
        string fileName = "Configs-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("configs/update-configs")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateConfig(long id)
    {
        var config = await _serviceManager.ConfigsService.GetConfigsByIdAsync<UpdateConfigsDto>(id: id);

        ViewData["UseLayout"] = true;

        config.CreateUserList = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: config.CreatedBy ?? 0);

        config.ModifiedUserList = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: config.ModifiedBy ?? 0);

        return View(config);
    }

    [HttpPost]
    [Route("configs/update-configs")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateConfig(UpdateConfigsDto config)
    {
        if (config.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;


            config.CreateUserList = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: config.CreatedBy ?? 0);

            config.ModifiedUserList = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: config.ModifiedBy ?? 0);

            return PartialView("UpdateConfig", config);
        }

        await _serviceManager.ConfigsService.UpdateConfigs(configs: config);

        TempData["SuccessToast"] = "Config updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }
}
