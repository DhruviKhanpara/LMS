using LMS.Application.Contracts.DTOs.BookFileMapping;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Books;

public class GetBookDto : BaseBookDto
{
    public long Id { get; set; }
    [Display(Name = "Genre name")]
    public string GenreName { get; set; } = null!;
    [Display(Name = "Status")]
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    [Display(Name = "Available copies")]
    public long AvailableCopies { get; set; }
    public bool IsRemoved { get; set; } = false;
    public List<GetBookFileMappingDto> BookFiles { get; set; } = new List<GetBookFileMappingDto>();
}
