namespace ExpenseTracker.Application.QueryParameters;
public sealed record WalletQueryParameters(
    string? Search,
    decimal? MinBalance,
    decimal? MaxBalance,
    string? SortBy);
