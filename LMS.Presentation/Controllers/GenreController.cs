using LMS.Application.Contracts.DTOs.Genre;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace LMS.Presentation.Controllers;

[Authorize]
public class GenreController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;

    public GenreController(IServiceManager serviceManager, IToastNotification toast)
    {
        _serviceManager = serviceManager;
        _toast = toast;
    }

    [HttpGet]
    [Route("genre/get-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetGenres(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null)
    {
        var paginatedGenres = await _serviceManager.GenreService.GetAllGenreAsync<GetGenreDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData);

        if (paginatedGenres.Data is null || !paginatedGenres.Data.Any())
            _toast.AddAlertToastMessage("No Genre data are available");

        return View(paginatedGenres);
    }

    [HttpGet]
    [Route(("genre/genre-detail"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetGenreDetailById(long id)
    {
        var genre = await _serviceManager.GenreService.GetGenreByIdAsync<GetGenreDto>(id: id);
        return View(genre);
    }

    [HttpGet]
    [Route("genre/export-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportGenres()
    {
        byte[] excelStream = await _serviceManager.GenreService.ExportGenreData();
        string fileName = "Genres-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("genre/add-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public IActionResult AddGenre()
    {
        var genre = new AddGenreDto();

        ViewData["UseLayout"] = true;
        return View(genre);
    }

    [HttpPost]
    [Route("genre/add-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddGenre(AddGenreDto genre)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;
            return PartialView("AddGenre", genre);
        }

        await _serviceManager.GenreService.AddGenreAsync(genre: genre);

        TempData["SuccessToast"] = "Genre added successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("genre/update-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateGenre(long id)
    {
        var genre = await _serviceManager.GenreService.GetGenreByIdAsync<UpdateGenreDto>(id: id);

        ViewData["UseLayout"] = true;
        return View(genre);
    }

    [HttpPost]
    [Route("genre/update-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateGenre(UpdateGenreDto genre)
    {
        if (genre.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;
            return PartialView("UpdateGenre", genre);
        }

        await _serviceManager.GenreService.UpdateGenreAsync(genre: genre);

        TempData["SuccessToast"] = "Genre updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("genre/remove-genre")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> DeleteGenre(long id)
    {
        await _serviceManager.GenreService.DeleteGenreAsync(id: id);

        _toast.AddSuccessToastMessage("Genre removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetGenres", "Genre");
        }
    }

    [HttpGet]
    [Route("genre/delete-genre")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteGenre(long id)
    {
        await _serviceManager.GenreService.PermanentDeleteGenreAsync(id: id);

        _toast.AddSuccessToastMessage("Genre deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetGenres", "Genre");
        }
    }
}
