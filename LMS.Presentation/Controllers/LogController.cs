using LMS.Application.Contracts.Interfaces.Services;
using LMS.Common.Models;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Presentation.Controllers;

public class LogController : Controller
{
    private readonly IServiceManager _serviceManager;

    public LogController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    [Route("log/audit-history/book")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetBookAuditHistoryByBookId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetBooksLogAsync(id: id, logOperation: logOperation, pageNumber: 1, pageSize: 20, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedBook"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/config")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetConfigsAuditHistoryByConfigId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetConfigsLogAsync(id: id, logOperation: logOperation, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedConfig"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/genre")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetGenreAuditHistoryByGenreId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetGenresLogAsync(id: id, logOperation: logOperation, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedGenre"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/membership")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetMembershipAuditHistoryByMembershipId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetMembershipsLogAsync(id: id, logOperation: logOperation, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedMembership"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/penalty")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetPenaltyAuditHistoryByPenaltyId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetPenaltiesLogAsync(id: id, logOperation: logOperation, pageNumber: 1, pageSize: 20, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedPenalty"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/reservation")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetReservationAuditHistoryByReservationId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetReservationsLogAsync(id: id, logOperation: logOperation, pageNumber: 1, pageSize: 20, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedReservation"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/transection")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetTransectionAuditHistoryByTransectionId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetTransectionsLogAsync(id: id, logOperation: logOperation, pageNumber: 1, pageSize: 20, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedTransection"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/user")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetUserAuditHistoryByUserId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetUsersLogAsync(id: id, logOperation: logOperation, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedUser"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/user-membership")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetUserMembershipAuditHistoryByUserMembershipId(long id, string? logOperation = null, bool needJson = false)
    {
        var paginatedLogs = await _serviceManager.LogService.GetUserMembershipsLogAsync(id: id, logOperation: logOperation, pageNumber: 1, pageSize: 20, startDate: DateTimeOffset.UtcNow.AddMonths(-1), endDate: DateTimeOffset.UtcNow);

        if (needJson)
            return Json(paginatedLogs.Data);
        else
        {
            ViewData["SelectedUserMembership"] = id;
            paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
                { "LogOpeartions", SelectListForLogOperation(includeDelete: false) }
            };

            return View(paginatedLogs);
        }
    }

    [HttpGet]
    [Route("log/audit-history/books")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetBooksAuditHistory(int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? id = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetBooksLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "BookOptions", await _serviceManager.BookService.GetAllBookSelectionAsync()},
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/configs")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetConfigsAuditHistory(int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? id = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetConfigsLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "ConfigOptions", await _serviceManager.ConfigsService.GetAllConfigsSelectionAsync() },
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/genres")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetGenresAuditHistory(int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? id = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetGenresLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "GenreOptions", await _serviceManager.GenreService.GetAllGenreSelectionAsync()},
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/memberships")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetMembershipsAuditHistory(int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? id = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetMembershipsLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "MembershipOptions", await _serviceManager.MembershipService.GetAllMembershipSelectionAsync()},
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/penalties")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetPenaltiesAuditHistory(long id, int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetPenaltiesLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/reservations")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetReservationsAuditHistory(long id, int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetReservationsLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/transections")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetTransectionsAuditHistory(long id, int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetTransectionsLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/users")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetUsersAuditHistory(int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? id = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetUsersLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "UserOptions", await _serviceManager.UserService.GetAllUserSelectionAsync() },
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    [HttpGet]
    [Route("log/audit-history/user-memberships")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> GetUserMembershipsAuditHistory(long id, int pageSize = 10, int pageNumber = 1, string? orderBy = null, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, string? logOperation = null, long? performedBy = null)
    {
        var paginatedLogs = await _serviceManager.LogService.GetUserMembershipsLogAsync(id: id, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, logOperation: logOperation, startDate: startDate, endDate: endDate, performedByUserId: performedBy);

        paginatedLogs.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "LogOpeartions", SelectListForLogOperation(includeDelete: false) },
            { "PerformedByOptions", await _serviceManager.UserService.GetAllUserSelectionAsync()},
        };

        return View(paginatedLogs);
    }

    private List<SelectListItem> SelectListForLogOperation(bool includeDelete = false)
    {
        var operations = new List<string>
        {
            LogOperationTypes.Insert,
            LogOperationTypes.Update,
            LogOperationTypes.SoftDelete,
            LogOperationTypes.Restore
        };

        if (includeDelete)
            operations.Add(LogOperationTypes.Delete);

        var list = operations
            .Select(op => new SelectListItem
            {
                Value = op,
                Text = op
            }).ToList();

        return list;
    }
}
