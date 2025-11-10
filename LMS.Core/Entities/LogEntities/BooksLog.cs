using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class BooksLog : LogAudit
{
    #region Table References
    public virtual Genre? Genre { get; set; }
    public virtual Status? Status { get; set; }
    #endregion

    public long? Id { get; set; }
    public long? GenreId { get; set; }
    public long? StatusId { get; set; }
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
