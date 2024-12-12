using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.QueryParameters;

public sealed record CategoryQueryParameters(
    string? Search,
    CategoryType? Type,
    string? SortBy = "name_asc");
