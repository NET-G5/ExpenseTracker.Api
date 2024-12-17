using ExpenseTracker.Application.Constants;

namespace ExpenseTracker.Application.Models;

public sealed class PaginatedList<T> : List<T>
{
    public int PagesCount { get; set; }
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public PaginatedList(IEnumerable<T> elements, int pageSize, int totalCount, int currentPage)
    {
        AddRange(elements);

        PagesCount = (int)Math.Ceiling((double)totalCount / pageSize); ;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        HasNextPage = currentPage < PagesCount;
        HasPreviousPage = currentPage > PaginationConstants.MIN_PAGE_NUMBER;
    }

    public PaginationMetadata Metadata =>
        new(PagesCount, TotalCount, CurrentPage, HasNextPage, HasPreviousPage);
}
