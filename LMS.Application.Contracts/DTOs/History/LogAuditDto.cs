namespace LMS.Application.Contracts.DTOs.Log;

public class LogAuditDto
{
    public long? Id { get; set; }
    public string? Operation { get; set; }
    public string? OperationColor { get; set; }
    public string? OperationBgColor { get; set; }
    public string? LogType { get; set; }
    public string? LogTypeColor { get; set; }
    public string? LogTypeBgColor { get; set; }
    public DateTimeOffset? LogTime { get; set; }
    public string? PerformedBy { get; set; }
}
