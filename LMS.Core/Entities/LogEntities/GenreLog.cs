using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class GenreLog : LogAudit
{
    public long? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
