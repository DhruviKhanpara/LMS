using LMS.Core.Common;

namespace LMS.Core.Entities;

public partial class Genre : Audit
{
    #region Table References
    public virtual ICollection<Books> Books { get; set; } = new List<Books>();
    #endregion

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
