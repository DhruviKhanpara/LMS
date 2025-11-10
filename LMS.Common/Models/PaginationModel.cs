namespace LMS.Common.Models;

public class PaginationModel
{
    public int PageNumber { get; private set; } = 1;
    public int PageSize { get; private set; } = 10;
    public int PageCount { get; private set; }
    public long TotalCount { get; private set; }

    public PaginationModel(int pageNumber, int pageSize, int totalCount)
    {
        PageCount = totalCount != 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 1;
        PageNumber = pageNumber <= PageCount ? pageNumber : 1;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public PaginationModel(int totalCount)
    {
        PageSize = totalCount;
        TotalCount = totalCount;
        PageCount = 1;
        PageNumber = 1;
    }
}
