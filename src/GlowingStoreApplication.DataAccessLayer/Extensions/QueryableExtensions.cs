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

        var list = await source.Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1).ToListAsync(cancellationToken);
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
}