using LMS.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Core.Entities;

public class BookFileMapping : Audit
{
    #region Table References
    [ForeignKey(nameof(BookId))]
    public virtual Books Books { get; set; } = null!;
    #endregion

    public long Id { get; set; }
    public long BookId { get; set; }
    public string Label { get; set; } = null!;
    public string fileLocation { get; set; } = null!;
}
