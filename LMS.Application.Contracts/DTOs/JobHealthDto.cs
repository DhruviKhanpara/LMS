namespace LMS.Application.Contracts.DTOs;

public class JobHealthDto
{
    public string JobId { get; set; } = null!;
    public string JobName { get; set; } = null!;
    public DateTime? LastExecution { get; set; }
    public DateTime? NextExecution { get; set; }
    public string LastState { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
    public TimeSpan? Duration { get; set; }
}
