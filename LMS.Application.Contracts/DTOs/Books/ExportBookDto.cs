using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Books;

public class ExportBookDto : BaseBookDto
{
    public long Id { get; set; }
    [Display(Name = "Genre name")]
    public string GenreName { get; set; } = null!;
    [Display(Name = "Status")]
    public string StatusLabel { get; set; } = null!;
    [Display(Name = "Available copies")]
    public long AvailableCopies { get; set; }
    [Display(Name = "Is Removed?")]
    public bool IsRemoved { get; set; } = false;
}
