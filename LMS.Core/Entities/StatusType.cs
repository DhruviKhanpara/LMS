namespace LMS.Core.Entities;

public class StatusType
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}
