namespace LMS.Application.Contracts.DTOs;

public class ActivityInfo
{
    public string Activity { get; set; } = null!;
    public DateTimeOffset ActivityDate { get; set; }
}

public class RecentActivityByPeople : ActivityInfo
{
    public string profilePhoto { get; set; } = null!;
    public string Username { get; set; } = null!;
}