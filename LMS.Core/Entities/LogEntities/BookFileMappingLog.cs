using LMS.Core.Common;

namespace LMS.Core.Entities.LogEntities;

public class BookFileMappingLog : LogAudit
{
    public long? Id { get; set; }
    public long? BookId { get; set; }
    public string? Label { get; set; }
    public string? fileLocation { get; set; }
}
