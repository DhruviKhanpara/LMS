using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class StatusLog : LogAudit
{
    public long? Id { get; set; }
    public long? StatusTypeId { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public new long? CreatedBy { get; set; }
}
