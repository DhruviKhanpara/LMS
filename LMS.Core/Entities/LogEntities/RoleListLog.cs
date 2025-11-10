using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class RoleListLog : LogAudit
{
    public long? Id { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
}
