using Microsoft.EntityFrameworkCore;

namespace GlowingStoreApplication.DataAccessLayer.Extensions;

public static class QueryableExtensions
{
    public static async Task<bool> HasNextPageAsync<T>(this IQueryable<T> source, int pageIndex, int itemsPerPage, CancellationToken cancellationToken = default) where T : class
    {
        var hasItems = await source.AnyAsync(cancellationToken);
        if (!hasItems)
        {
            return false;
        }

        var skip = Skip(pageIndex, itemsPerPage);
        var take = Take(itemsPerPage);

        var list = await source.Skip(skip).Take(take).ToListAsync(cancellationToken);
        return list.Count > itemsPerPage;
    }

    public static async Task<int> TotalPagesAsync<T>(this IQueryable<T> source, int itemsPerPage, CancellationToken cancellationToken = default) where T : class
    {
        var hasItems = await source.AnyAsync(cancellationToken);
        if (!hasItems)
        {
            return 0;
        }

        var totalCount = await source.LongCountAsync(cancellationToken);
        var totalPages = Convert.ToInt32(totalCount / itemsPerPage);

        if ((totalPages % itemsPerPage) > 0)
        {
            totalPages++;
        }

        return totalPages;
    }

    private static int Skip(int pageIndex, int itemsPerPage) => pageIndex * itemsPerPage;

    private static int Take(int itemsPerPage) => itemsPerPage + 1;
}