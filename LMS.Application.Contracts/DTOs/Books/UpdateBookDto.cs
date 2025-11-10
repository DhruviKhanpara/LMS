using LMS.Application.Contracts.DTOs.BookFileMapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMS.Application.Contracts.DTOs.Books;

public class UpdateBookDto : BaseBookDto
{
    public long Id { get; set; }
    public long GenreId { get; set; }
    public long StatusId { get; set; }
    public long? AvailableCopies { get; set; }
    public IFormFile? CoverPage { get; set; }
    public IFormFile? BookPreview { get; set; }
    public bool IsDeletedCoverPage { get; set; }
    public bool IsDeletedBookPreview { get; set; }
    public List<GetBookFileMappingDto> BookFiles { get; set; } = new List<GetBookFileMappingDto>();
    public IEnumerable<SelectListItem> GenreList { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> BookStatusList { get; set; } = new List<SelectListItem>();
}
