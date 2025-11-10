using LMS.Application.Contracts.DTOs.BookFileMapping;

namespace LMS.Application.Contracts.DTOs.Books;

public class GetBookInfoDto : BaseBookDto
{
    public long Id { get; set; }
    public string GenreName { get; set; } = null!;
    public string StatusLabel { get; set; } = null!;
    public string StatusLabelColor { get; set; } = null!;
    public string StatusLabelBgColor { get; set; } = null!;
    public long AvailableCopies { get; set; }
    public long TotalBorrowing { get; set; }
    public long TotalReservation { get; set; }
    public long ActiveReservation { get; set; }
    public bool? canBorrow { get; set; }
    public bool? canReserve { get; set; }
    public bool IsBorrowed { get; set; }
    public bool IsReserved { get; set; }
    public bool IsAllocated { get; set; }
    public List<RecentActivityByPeople> RecentActivitiesByPeople { get; set; } = new List<RecentActivityByPeople>();
    public List<GetBookFileMappingDto> BookFiles { get; set; } = new List<GetBookFileMappingDto>();
}