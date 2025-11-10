namespace LMS.Common.Helpers;

public static class PaginationExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageSize, int pageNumber)
    {
        if (pageSize > 0 && pageNumber > 0)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
        return query;
    }
}
