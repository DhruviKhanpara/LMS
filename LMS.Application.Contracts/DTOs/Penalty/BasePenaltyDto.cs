namespace LMS.Application.Contracts.DTOs.Penalty;

public class BasePenaltyDto
{
    public long UserId { get; set; }
    public long? TransectionId { get; set; }
    public long StatusId { get; set; }
    public long PenaltyTypeId { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public int? OverDueDays { get; set; }
}
