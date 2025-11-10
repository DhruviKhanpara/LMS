namespace LMS.Application.Contracts.DTOs.Configs;

public class GetConfigsDto
{
    public long Id { get; set; }
    public string KeyName { get; set; } = null!;
    public string KeyValue { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsRemoved { get; set; }
}
