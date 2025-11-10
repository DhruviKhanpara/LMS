using LMS.Application.Contracts.DTOs.Home;

namespace LMS.Application.Contracts.Interfaces.Services;

public interface IHomeService
{
    Task<List<ChartData>> GenreVisualization(bool? forLogin = null, int? visualizingMonth = null);
    Task<UserDashboardDto> UserDashboardDataAync();
    Task<LibrarianDashboardDto> LibrarianDashboardDataAync();
    Task<AdminDashboardDto> AdminDashboardDataAync();
}
