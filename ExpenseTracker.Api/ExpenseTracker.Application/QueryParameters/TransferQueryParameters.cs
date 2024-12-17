using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.QueryParameters;

public sealed record TransferQueryParameters(
    string? Search,
    decimal? MinAmount,
    decimal? MaxAmount,
    DateOnly? MinDate,
    DateOnly? MaxDate,
    CategoryType? Type,
    string SortBy = "date_desc",
    int PageSize = 25,
    int PageNumber = 1);
