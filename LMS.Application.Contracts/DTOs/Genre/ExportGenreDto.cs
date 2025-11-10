using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Genre;

public class ExportGenreDto : BaseGenreDto
{
    public long Id { get; set; }
    [Display(Name = "Total Active Book Count")]
    public long TotalActiveBooks { get; set; }
    public bool IsRemoved { get; set; } = false;
}
