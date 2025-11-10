using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class ConfigsLog : LogAudit
{
    public long? Id { get; set; }
    public string? KeyName { get; set; }
    public string? KeyValue { get; set; }
    public string? Description { get; set; }
}
