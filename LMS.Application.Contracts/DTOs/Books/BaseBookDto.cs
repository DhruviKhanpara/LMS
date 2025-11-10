using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Contracts.DTOs.Books;

public class BaseBookDto
{
    public string Title { get; set; } = null!;
    [Display(Name = "Book Description")]
    public string BookDescription { get; set; } = null!;
    public string Isbn { get; set; } = null!;
    public string Author { get; set; } = null!;
    [Display(Name = "Author Description")]
    public string AuthorDescription { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    [Display(Name = "Publish date")]
    public DateTimeOffset? PublishAt { get; set; }
    [Display(Name = "Total copies")]
    public long? TotalCopies { get; set; }
    public decimal? Price { get; set; }
}
