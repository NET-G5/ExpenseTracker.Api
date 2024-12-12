using ExpenseTracker.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Extensions;

internal static class QueryableExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);

        var result = await source
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(result, pageSize, totalCount, pageNumber);
    }
}
