using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Books;

public class AddBookDto : BaseBookDto
{
    public long GenreId { get; set; }
    public long StatusId { get; set; }
    public IFormFile CoverPage { get; set; } = null!;
    public IFormFile? BookPreview { get; set; }
    public IEnumerable<SelectListItem> GenreList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> BookStatusList { get; set; } = new List<SelectListItem>();
}
