namespace LMS.Application.Contracts.DTOs.Home;

public class UserDashboardDto
{
    public string User { get; set; } = null!;
    public string MembershipLabelPopOverText { get; set; } = null!;
    public string? MembreshipLabel { get; set; }
    public long BorrowLimit { get; set; }
    public long ReservationLimit { get; set; }
    public decimal UnPaidPenaltyAmount { get; set; }
    public List<RecentCheckOuts> RecentCheckOuts { get; set; } = new List<RecentCheckOuts>();
    public List<ChartData> GenreVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> ReservationVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> CheckoutsVisualization { get; set; } = new List<ChartData>();
    public List<NotificationData> Notifications { get; set; } = new List<NotificationData>();
}
