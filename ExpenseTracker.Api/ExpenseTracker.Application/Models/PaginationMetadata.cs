namespace ExpenseTracker.Application.Models;

public sealed record PaginationMetadata(
    int PagesCount,
    int TotalCount,
    int CurrentPage,
    bool HasNextPage,
    bool HasPreviousPage);
