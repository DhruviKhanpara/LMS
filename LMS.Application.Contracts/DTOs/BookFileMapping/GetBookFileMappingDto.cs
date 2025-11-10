namespace LMS.Application.Contracts.DTOs.BookFileMapping;

public class GetBookFileMappingDto
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public string fileLocation { get; set; } = null!;
    //public bool IsActive { get; set; } = true;
}
