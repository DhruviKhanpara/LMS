using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class PenaltyTypeLog : LogAudit
{
    public long? Id { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
}
