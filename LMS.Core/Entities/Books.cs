using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public partial class Books : Audit
{
    #region Table References
    [ForeignKey(nameof(GenreId))]
    public virtual Genre Genre { get; set; } = null!;
    [ForeignKey(nameof(StatusId))]
    public virtual Status Status { get; set; } = null!;
    public virtual List<BookFileMapping> BookFileMappings { get; set; } = new List<BookFileMapping>();
    public virtual List<Transection> Transections { get; set; } = new List<Transection>();
    public virtual List<Reservation> Reservations { get; set; } = new List<Reservation>();
    #endregion

    public long Id { get; set; }
    public long GenreId { get; set; }
    public long StatusId { get; set; }
    public string Title { get; set; } = null!;
    public string BookDescription { get; set; } = null!;
    public decimal Price { get; set; }
    public string Isbn { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string AuthorDescription { get; set; } = null!; 
    public string Publisher { get; set; } = null!;
    public DateTimeOffset PublishAt { get; set; }
    public long TotalCopies { get; set; }
    public long AvailableCopies { get; set; }
}
