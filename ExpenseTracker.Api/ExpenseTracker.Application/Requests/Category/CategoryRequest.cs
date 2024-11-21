namespace ExpenseTracker.Application.Requests.Category;

public sealed record CategoryRequest(
    Guid UserId,
    int CategoryId);
