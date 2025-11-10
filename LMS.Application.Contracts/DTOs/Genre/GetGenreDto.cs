using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Genre;

public class GetGenreDto
{
    public long Id { get; set; }
    [Display(Name = "Genre Name")]
    public string Name { get; set; } = null!;
    [Display(Name = "Genre Description")]
    public string Description { get; set; } = null!;
    [Display(Name = "Total Active Book Count")]
    public long TotalActiveBooks { get; set; }
    public bool IsRemoved { get; set; } = false;
}
