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
public class TransectionController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;
    private readonly HttpContext _httpContext;

    public TransectionController(IServiceManager serviceManager, IToastNotification toast, IHttpContextAccessor httpContextAccessor)
    {
        _serviceManager = serviceManager;
        _toast = toast;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    [HttpGet]
    [Route("transection/get-transection")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetTransections(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null, long? user = null, long? book = null)
    {
        await _serviceManager.PenaltyService.CalculatePenaltyForHoldingBooks();

        var paginatedTransection = await _serviceManager.TransectionService.GetAllTransectionAsync<GetTransectionDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData, userId: user, bookId: book);

        paginatedTransection.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "Users", await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User }) },
            { "Books", await _serviceManager.BookService.GetAllBookSelectionAsync(bookStatusList: Enum.GetValues(typeof(BookStatusEnum)).Cast<long>().ToArray()) }
        };

        if (paginatedTransection.Data is null || !paginatedTransection.Data.Any())
            _toast.AddAlertToastMessage("No Book transection data are available");

        return View(paginatedTransection);
    }

    [HttpGet]
    [Route("transection/get-user-transection")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> GetUserTransections(int pageNumber = 1, int pageSize = 5)
    {
        await _serviceManager.PenaltyService.CalculatePenaltyForHoldingBooks(forLogin: true);

        var paginatedTransection = await _serviceManager.TransectionService.GetUserTransectionAsync(pageSize: pageSize, pageNumber: pageNumber);

        if (paginatedTransection.Data is null || !paginatedTransection.Data.Any())
            _toast.AddAlertToastMessage("No active book transection data are available");

        if (paginatedTransection.Data1 is null || !paginatedTransection.Data1.Any())
            _toast.AddAlertToastMessage("No past book transection data are available");

        return View(paginatedTransection);
    }


    [HttpGet]
    [Route("transection/get-transection-by-user")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetTransectionsByUser(long user)
    {
        var paginatedTransection = await _serviceManager.TransectionService.GetAllTransectionAsync<GetTransectionDto>(isActive: true, userId: user);

        if (paginatedTransection.Data is null || !paginatedTransection.Data.Any())
            _toast.AddAlertToastMessage("No transection data are available");

        return Json(paginatedTransection.Data);
    }

    [HttpGet]
    [Route("transection/transection-details")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin, RoleListEnum.User)]
    public async Task<IActionResult> GetTransectionDetailById(long id)
    {
        var transection = await _serviceManager.TransectionService.GetTransectionByIdAsync<GetTransectionDto>(id: id);

        var isLogin = long.TryParse(_httpContext!.GetUserId(), out long authUserId);
        var authUserRole = _httpContext!.GetUserRole();

        if (authUserRole.Equals(nameof(RoleListEnum.User), StringComparison.InvariantCultureIgnoreCase) && transection.UserId != authUserId)
        {
            TempData["ErrorToast"] = "Have not permission of this Transection view";
            return Json(new { success = false, visit = Request.Headers["Referer"].ToString() });
        }

        return View(transection);
    }

    [HttpGet]
    [Route("transection/export-transection")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportTransections()
    {
        byte[] excelStream = await _serviceManager.TransectionService.ExportTransectionData();
        string fileName = "Transections-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("transection/borrow-book-for-login-user")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> BorrowBookForLogin(long bookId)
    {
        await _serviceManager.PenaltyService.CalculatePenaltyForHoldingBooks(forLogin: true);
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.TransectionService.BorrowBookforLoginUserAsync(bookId: bookId);
            _toast.AddSuccessToastMessage("Book borrow successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/borrow-book-for-user")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> BorrowBook(long bookId, long userId)
    {
        await _serviceManager.TransectionService.BorrowBookforLoginUserAsync(bookId: bookId, userId: userId);
        _toast.AddSuccessToastMessage("Book borrow successfully");

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/add-transection")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddTransection()
    {
        var transection = new AddTransectionDto();
        ViewData["UseLayout"] = true;

        transection.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });
        transection.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookStatusList: new[] { (long)BookStatusEnum.Available });

        transection.TransectionStatusList = Enum.GetValues(typeof(TransectionStatusEnum))
            .Cast<TransectionStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        return View(transection);
    }

    [HttpPost]
    [Route("transection/add-transection")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddTransection(AddTransectionDto transection)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            transection.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });
            transection.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookStatusList: new[] { (long)BookStatusEnum.Available });

            transection.TransectionStatusList = Enum.GetValues(typeof(TransectionStatusEnum))
            .Cast<TransectionStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

            return PartialView("AddTransection", transection);
        }

        await _serviceManager.TransectionService.AddTransectionAsync(transection: transection);

        TempData["SuccessToast"] = "Transection added successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("transection/update-transection")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateTransection(long id)
    {
        var transection = await _serviceManager.TransectionService.GetTransectionByIdAsync<UpdateTransectionDto>(id: id);

        ViewData["UseLayout"] = true;

        transection.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: transection.UserId);
        transection.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookId: transection.BookId);

        transection.TransectionStatusList = Enum.GetValues(typeof(TransectionStatusEnum))
            .Cast<TransectionStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        return View(transection);
    }

    [HttpPost]
    [Route("transection/update-transection")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateTransection(UpdateTransectionDto transection)
    {
        if (transection.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            transection.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: transection.UserId);
            transection.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookId: transection.BookId);

            transection.TransectionStatusList = Enum.GetValues(typeof(TransectionStatusEnum))
            .Cast<TransectionStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

            return PartialView("UpdateTransection", transection);
        }

        await _serviceManager.TransectionService.UpdateTransectionAsync(transection: transection);

        TempData["SuccessToast"] = "Transection updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("transection/remove-user-transection")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> RemoveUserTransection(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.TransectionService.TransectionActionsAsync(id: id, transectionAction: TransectionActionEnum.Delete);
            _toast.AddSuccessToastMessage("Transection removed successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/remove-transection")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> RemoveTransection(long id)
    {
        await _serviceManager.TransectionService.TransectionActionsAsync(id: id, transectionAction: TransectionActionEnum.Delete);
        _toast.AddSuccessToastMessage("Transection removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetTransections", "Transection");
        }
    }

    [HttpGet]
    [Route("transection/return-user-transection")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> ReturnBorrowBook(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.TransectionService.TransectionActionsAsync(id: id, transectionAction: TransectionActionEnum.Return);
            _toast.AddSuccessToastMessage("Book return successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/renew-user-transection")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> RenewBorrowBook(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.TransectionService.TransectionActionsAsync(id: id, transectionAction: TransectionActionEnum.Renew);
            _toast.AddSuccessToastMessage("Book renew successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/cancel-user-transection")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> CancelBorrowBook(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.TransectionService.TransectionActionsAsync(id: id, transectionAction: TransectionActionEnum.Cancel);
            _toast.AddSuccessToastMessage("Transection cancel successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/claimLost-user-transection")]
    [AuthorizeRoles(RoleListEnum.User, RoleListEnum.Admin)]
    public async Task<IActionResult> ClaimLostBorrowBook(long id)
    {
        await _serviceManager.TransectionService.TransectionActionsAsync(id: id, transectionAction: TransectionActionEnum.ClaimLost);
        _toast.AddSuccessToastMessage("Book claim to lost is successfully");
        
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("transection/delete-user-transection")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteTransection(long id)
    {
        await _serviceManager.TransectionService.PermanentDeleteTransectionAsync(id: id);

        _toast.AddSuccessToastMessage("Transection deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetTransection", "Transection");
        }
    }
}
