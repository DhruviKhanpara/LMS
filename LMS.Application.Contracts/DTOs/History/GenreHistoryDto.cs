namespace LMS.Application.Contracts.DTOs.Log;

public class GenreHistoryDto : LogAuditDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
