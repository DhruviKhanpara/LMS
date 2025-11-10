using LMS.Application.Contracts.DTOs.Books;
using LMS.Application.Contracts.Interfaces.Services;
using LMS.Core.Enums;
using LMS.Presentation.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace LMS.Presentation.Controllers;

public class BookController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IToastNotification _toast;

    public BookController(IServiceManager serviceManager, IToastNotification toast)
    {
        _serviceManager = serviceManager;
        _toast = toast;
    }

    [HttpGet]
    [Route("book/get-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetBooks(int pageNumber = 1, int pageSize = 10, string? orderBy = null, bool? activeData = null, long? genre = null)
    {
        var paginatedBooks = await _serviceManager.BookService.GetAllBookAsync<GetBookDto>(pageSize: pageSize, pageNumber: pageNumber, orderBy: orderBy, isActive: activeData, genreId: genre);
        
        paginatedBooks.Selections = new Dictionary<string, List<SelectListItem>?>() {
            { "Genres", await _serviceManager.GenreService.GetAllGenreSelectionAsync() }
        };

        if (paginatedBooks.Data is null || !paginatedBooks.Data.Any())
            _toast.AddAlertToastMessage("No Book data are available");

        return View(paginatedBooks);
    }

    [HttpGet]
    [Route("book/get-book-page")]
    public async Task<IActionResult> GetBookPage(string nameStartWith = "A", int pageNumber = 1, int pageSize = 14, int? latestDataCount = null)
    {
        var paginatedBooks = await _serviceManager.BookService.GetAllBookByNameFilterAsync(nameStartWith: nameStartWith, pageNumber: pageNumber, pageSize: pageSize, latestDataCount: latestDataCount);

        if ((paginatedBooks.Data is null || !paginatedBooks.Data.Any()) && (paginatedBooks.Data1 is null || !paginatedBooks.Data1.Any()))
            _toast.AddAlertToastMessage("No Book data are available");

        return View(paginatedBooks);
    }

    [HttpGet]
    [Route(("book/book-detail"))]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetBookDetailById(long id)
    {
        var book = await _serviceManager.BookService.GetBookByIdAsync<GetBookDto>(id: id);
        return View(book);
    }

    [HttpGet]
    [Route(("book/book-info"))]
    [AuthorizeRoles(RoleListEnum.User)]
    public async Task<IActionResult> GetBookInfoById(long id)
    {
        await _serviceManager.ReservationService.ReallocateExpiredAllocationToReservationAsync(forLogin: true, notifyUser: true);
        await _serviceManager.ReservationService.AllocateBookToReservation(forLogin: true, notifyUser: true);

        await _serviceManager.PenaltyService.CalculatePenaltyForHoldingBooks(forLogin: true);
        var book = await _serviceManager.BookService.GetBookInfoByIdAsync(id: id);

        return View(book);
    }

    [HttpGet]
    [Route("book/export-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> GetExportBooks()
    {
        byte[] excelStream = await _serviceManager.BookService.ExportBookData();
        string fileName = "Books-" + DateTime.Now.Ticks + ".xlsx";

        return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet]
    [Route("book/add-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddBook()
    {
        var book = new AddBookDto();

        ViewData["UseLayout"] = true;

        book.GenreList = await _serviceManager.GenreService.GetAllGenreSelectionAsync();

        book.BookStatusList = Enum.GetValues(typeof(BookStatusEnum))
            .Cast<BookStatusEnum>()
            .Where(x => !new[] { BookStatusEnum.Removed }.Contains(x))
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        return View(book);
    }

    [HttpPost]
    [Route("book/add-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> AddBook(AddBookDto book)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            book.GenreList = await _serviceManager.GenreService.GetAllGenreSelectionAsync();

            book.BookStatusList = Enum.GetValues(typeof(BookStatusEnum))
                .Cast<BookStatusEnum>()
            .Where(x => !new[] { BookStatusEnum.Removed }.Contains(x))
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            return PartialView("AddBook", book);
        }

        await _serviceManager.BookService.AddBookAsync(book: book);

        TempData["SuccessToast"] = "Book added successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("book/update-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateBook(long id)
    {
        var book = await _serviceManager.BookService.GetBookByIdAsync<UpdateBookDto>(id: id);

        ViewData["UseLayout"] = true;

        book.GenreList = await _serviceManager.GenreService.GetAllGenreSelectionAsync();

        book.BookStatusList = Enum.GetValues(typeof(BookStatusEnum))
            .Cast<BookStatusEnum>()
            .Where(x => !new[] { BookStatusEnum.Removed }.Contains(x))
            .Select(item => new SelectListItem
            {
                Value = ((long)item).ToString(),
                Text = item.ToString()
            }).ToList();

        return View(book);
    }

    [HttpPost]
    [Route("book/update-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> UpdateBook(UpdateBookDto book)
    {
        if (book.Id <= 0)
        {
            TempData["ErrorToast"] = "Invalid action has been found";
            return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
        }

        if (!ModelState.IsValid)
        {
            ViewData["UseLayout"] = false;

            book.GenreList = await _serviceManager.GenreService.GetAllGenreSelectionAsync();

            book.BookStatusList = Enum.GetValues(typeof(BookStatusEnum))
                .Cast<BookStatusEnum>()
                .Where(x => !new[] { BookStatusEnum.Removed }.Contains(x))
                .Select(item => new SelectListItem
                {
                    Value = ((long)item).ToString(),
                    Text = item.ToString()
                }).ToList();

            book.CoverPage = book.BookPreview = null;
            return PartialView("UpdateBook", book);
        }

        await _serviceManager.BookService.UpdateBookAsync(book: book);

        TempData["SuccessToast"] = "Book updated successfully";
        return Json(new { success = true, visit = Request.Headers["Referer"].ToString() });
    }

    [HttpGet]
    [Route("book/remove-book")]
    [AuthorizeRoles(RoleListEnum.Librarian, RoleListEnum.Admin)]
    public async Task<IActionResult> DeleteBook(long id)
    {
        await _serviceManager.BookService.DeleteBookAsync(id: id);
        await _serviceManager.ReservationService.RemoveBookReservationAsync(bookId: id);

        _toast.AddSuccessToastMessage("Book removed successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetBooks", "Book");
        }
    }

    [HttpGet]
    [Route("book/delete-book")]
    [AuthorizeRoles(RoleListEnum.Admin)]
    public async Task<IActionResult> PermanentDeleteBook(long id)
    {
        await _serviceManager.BookService.PermanentDeleteBookAsync(id: id);

        _toast.AddSuccessToastMessage("Book deleted successfully");

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && Uri.IsWellFormedUriString(referer, UriKind.RelativeOrAbsolute))
        {
            return Redirect(referer);
        }
        else
        {
            return RedirectToAction("GetBooks", "Book");
        }
    }
}