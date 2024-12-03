using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Requests.Category;

public record CreateCategoryRequest(
    string Name,
    string? Description,
    CategoryType Type);
