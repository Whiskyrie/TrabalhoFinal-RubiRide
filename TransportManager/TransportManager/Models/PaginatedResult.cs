// Models/PaginatedResult.cs
namespace TransportManager.Models;

public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int TotalCount { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex < TotalPages - 1;

    public PaginatedResult(List<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public static async Task<PaginatedResult<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize)
    {
        var totalCount = await source.CountAsync();
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedResult<T>(items, totalCount, pageIndex, pageSize);
    }
}