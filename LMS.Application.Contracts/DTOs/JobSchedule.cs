namespace LMS.Application.Contracts.DTOs;

public class JobSchedule
{
    public string Name { get; set; } = null!;
    public string FrequencyType { get; set; } = null!;
    public int? Interval { get; set; }
    public string? Time { get; set; }
}
