using LMS.Application.Contracts.DTOs.Reservation;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace LMS.Presentation.Controllers;

[Authorize]
public class ReservationController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;

    public ReservationController(IServiceManager serviceManager, IToastNotification toast)
    {
        _serviceManager = serviceManager;
        _toast = toast;
    }

    [HttpGet]
    [Route(("reservation/get-reservation"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetReservations(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null, long? user = null, long? book = null)
    {
        await _serviceManager.ReservationService.ReallocateExpiredAllocationToReservationAsync(notifyUser: true);
        await _serviceManager.ReservationService.AllocateBookToReservation(notifyUser: true);

        var paginatedReservation = await _serviceManager.ReservationService.GetAllReservationAsync<GetReservationDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData, userId: user, bookId: book);

        paginatedReservation.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "Users", await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User }) },
            { "Books", await _serviceManager.BookService.GetAllBookSelectionAsync(bookStatusList: Enum.GetValues(typeof(BookStatusEnum)).Cast<long>().ToArray()) }
        };

        if (paginatedReservation.Data is null || !paginatedReservation.Data.Any())
            _toast.AddAlertToastMessage("No Book reservation data are available");

        return View(paginatedReservation);
    }

    [HttpGet]
    [Route(("reservation/get-user-reservation"))]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> GetUserReservations(int pageNumber = 1, int pageSize = 5)
    {
        await _serviceManager.ReservationService.ReallocateExpiredAllocationToReservationAsync(forLogin: true, notifyUser: true);
        await _serviceManager.ReservationService.AllocateBookToReservation(forLogin: true, notifyUser: true);

        var paginatedReservation = await _serviceManager.ReservationService.GetUserReservationAsync(pageSize: pageSize, pageNumber: pageNumber);

        if (paginatedReservation.Data is null || !paginatedReservation.Data.Any())
            _toast.AddAlertToastMessage("No active book reservation data are available");

        if (paginatedReservation.Data1 is null || !paginatedReservation.Data1.Any())
            _toast.AddAlertToastMessage("No past book reservation data are available");

        return View(paginatedReservation);
    }

    [HttpGet]
    [Route("reservation/reservation-details")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetReservationDetailById(long id)
    {
        var reservation = await _serviceManager.ReservationService.GetReservationByIdAsync<GetReservationDto>(id: id);
        return View(reservation);
    }

    [HttpGet]
    [Route("reservation/export-reservation")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportReservation()
    {
        byte[] excelStream = await _serviceManager.ReservationService.ExportReservationData();
        string fileName = "Reservations-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route(("reservation/reserve-book-for-login-user"))]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> ReserveBookForLogin(long bookId)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.ReservationService.ReserveBookforLoginUserAsync(bookId: bookId);
            _toast.AddSuccessToastMessage("Book reserved successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("reservation/add-reservation")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddReservation()
    {
        var reservation = new AddReservationDto();
        ViewData["UseLayout"] = true;

        reservation.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });
        reservation.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookStatusList: Enum.GetValues(typeof(BookStatusEnum)).Cast<long>().Where(x => x != (long)BookStatusEnum.Removed).ToArray());

        reservation.ReservationStatusList = Enum.GetValues(typeof(ReservationsStatusEnum))
            .Cast<ReservationsStatusEnum>()
            .Where(x => !new[] { ReservationsStatusEnum.Allocated }.Contains(x))
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        return View(reservation);
    }

    [HttpPost]
    [Route("reservation/add-reservation")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddReservation(AddReservationDto reservation)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            reservation.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userRoleList: new[] { (long)RoleListEnum.User });
            reservation.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookStatusList: Enum.GetValues(typeof(BookStatusEnum)).Cast<long>().Where(x => x != (long)BookStatusEnum.Removed).ToArray());

            reservation.ReservationStatusList = Enum.GetValues(typeof(ReservationsStatusEnum))
            .Cast<ReservationsStatusEnum>()
            .Where(x => !new[] { ReservationsStatusEnum.Allocated }.Contains(x))
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

            return PartialView("AddReservation", reservation);
        }

        await _serviceManager.ReservationService.AddReservationAsync(reservation: reservation);

        TempData["SuccessToast"] = "Reservation added successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("reservation/update-reservation")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateReservation(long id)
    {
        var reservation = await _serviceManager.ReservationService.GetReservationByIdAsync<UpdateReservationDto>(id: id);
        ViewData["UseLayout"] = true;

        reservation.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: reservation.UserId);
        reservation.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookId: reservation.BookId);

        reservation.ReservationStatusList = Enum.GetValues(typeof(ReservationsStatusEnum))
            .Cast<ReservationsStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString(),
            }).ToList();

        return View(reservation);
    }

    [HttpPost]
    [Route("reservation/update-reservation")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateReservation(UpdateReservationDto reservation)
    {
        if (reservation.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            reservation.Users = await _serviceManager.UserService.GetAllUserSelectionAsync(userId: reservation.UserId);
            reservation.Books = await _serviceManager.BookService.GetAllBookSelectionAsync(bookId: reservation.BookId);

            reservation.ReservationStatusList = Enum.GetValues(typeof(ReservationsStatusEnum))
            .Cast<ReservationsStatusEnum>()
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString(),
                Disabled = item.ToString() == nameof(ReservationsStatusEnum.Allocated)
            }).ToList();

            return PartialView("UpdateReservation", reservation);
        }

        await _serviceManager.ReservationService.UpdateReservationAsync(reservation: reservation);

        TempData["SuccessToast"] = "Reservation updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("reservation/remove-user-reservation")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> RemoveUserReservation(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.ReservationService.ReservationActionsAsync(id: id, reservationAction: ReservationActionEnum.Delete);
            _toast.AddSuccessToastMessage("Reservation removed successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("reservation/remove-reservation")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> RemoveReservation(long id)
    {
        await _serviceManager.ReservationService.ReservationActionsAsync(id: id, reservationAction: ReservationActionEnum.Delete);
        _toast.AddSuccessToastMessage("Reservation removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetReservations", "Reservation");
        }
    }

    [HttpGet]
    [Route("reservation/cancel-user-reservation")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> CancelUserReservation(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.ReservationService.ReservationActionsAsync(id: id, reservationAction: ReservationActionEnum.Cancel);
            _toast.AddSuccessToastMessage("Reservation cancel successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("reservation/transfer-user-reservation")]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> TransferUserReservation(long id)
    {
        if (await _serviceManager.PenaltyService.HavePendingPenalty())
        {
            _toast.AddErrorToastMessage("You have unpaid penalty first pay it");
        }
        else
        {
            await _serviceManager.ReservationService.ReservationActionsAsync(id: id, reservationAction: ReservationActionEnum.Transfer);
            _toast.AddSuccessToastMessage("Reservation transfer successfully");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpGet]
    [Route("reservation/delete-user-reservation")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteReservation(long id)
    {
        await _serviceManager.ReservationService.PermanentDeleteReservationAsync(id: id);

        _toast.AddSuccessToastMessage("Reservation deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetReservation", "Reservation");
        }
    }
}
