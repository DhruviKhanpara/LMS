namespace LMS.Application.Contracts.DTOs.Log;

public class BooksHistoryDto : LogAuditDto
{
    public string? GenreName { get; set; }
    public string? StatusLabel { get; set; }
    public string? StatusLabelColor { get; set; }
    public string? StatusLabelBgColor { get; set; }
    public string? Title { get; set; }
    public string? BookDescription { get; set; }
    public decimal? Price { get; set; }
    public string? Isbn { get; set; }
    public string? Author { get; set; }
    public string? AuthorDescription { get; set; }
    public string? Publisher { get; set; }
    public DateTimeOffset? PublishAt { get; set; }
    public long? TotalCopies { get; set; }
    public long? AvailableCopies { get; set; }
}
