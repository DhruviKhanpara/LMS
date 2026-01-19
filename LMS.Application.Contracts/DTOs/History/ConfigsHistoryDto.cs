namespace LMS.Application.Contracts.DTOs.Log;

public class ConfigsHistoryDto : LogAuditDto
{
    public string? KeyName { get; set; }
    public string? KeyValue { get; set; }
    public string? Description { get; set; }
}
