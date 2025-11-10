namespace LMS.Application.Contracts.DTOs.Home;

public class AdminDashboardDto
{
    public string User { get; set; } = null!;
    public long TotalActiveMembership { get; set; }
    public long TotalActiveCheckout { get; set; }
    public long TotalOverdueCheckout { get; set; }
    public long TotalLostBooksFromCheckouts { get; set; }
    public long TotalCheckout { get; set; }
    public long TotalActiveReservation { get; set; }
    public long TotalReservation { get; set; }
    public decimal UnPaidPenaltyAmount { get; set; }
    public List<RecentCheckOuts> RecentCheckOuts { get; set; } = new List<RecentCheckOuts>();
    public List<OverdueCheckOuts> OverdueCheckouts { get; set; } = new List<OverdueCheckOuts>();
    public List<ChartData> CheckoutsVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> ReturnVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> LostVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> ReservationVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> CancelVisualization { get; set; } = new List<ChartData>();
    public List<ChartData> GenreVisualization { get; set; } = new List<ChartData>();
}
