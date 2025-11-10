namespace LMS.Core.Entities;

public class OutboxMessage
{
    public long Id { get; set; }
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public int RetryCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public long CreatedBy { get; set; }
    public bool IsProcessed { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public DateTimeOffset? NextAttemptAt { get; set; }
    public bool IsActive { get; set; }
}
