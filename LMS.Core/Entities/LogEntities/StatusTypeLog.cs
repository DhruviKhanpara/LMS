namespace LMS.Core.Entities.LogEntities;

public class StatusTypeLog
{
    public long SerialNumber { get; set; }
    public string? Operation { get; set; }
    public string? LogType { get; set; }
    public DateTimeOffset? LogTime { get; set; }
    public long? Id { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
