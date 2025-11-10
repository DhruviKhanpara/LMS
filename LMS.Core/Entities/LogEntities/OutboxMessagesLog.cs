namespace LMS.Core.Entities.LogEntities;

public class OutboxMessagesLog
{
    public long SerialNumber { get; set; }
    public string? Operation { get; set; }
    public string? LogType { get; set; }
    public DateTimeOffset? LogTime { get; set; }
    public long? Id { get; set; }
    public string? Type { get; set; }
    public string? Payload { get; set; }
    public int? RetryCount { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public bool? IsProcessed { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public DateTimeOffset? NextAttemptAt { get; set; }
    public bool? IsActive { get; set; }
}
