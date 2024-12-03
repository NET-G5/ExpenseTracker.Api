using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Requests.Category;

public sealed record UpdateCategoryRequest(
    int Id,
    string Name,
    string? Description,
    CategoryType Type)
    : CreateCategoryRequest(
        Name,
        Description,
        Type);
