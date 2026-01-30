namespace LMS.Application.Contracts.DTOs.UserMembershipMapping;

public class GetUserMembersipListDto : GetUserMembershipDto
{
    public string? UserProfilePhoto { get; set; }
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    public bool IsRemoved { get; set; }
}
