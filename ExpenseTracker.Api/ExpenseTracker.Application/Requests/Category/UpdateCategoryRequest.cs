using ExpenseTracker.Application.Requests.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Requests.Category;

public sealed record UpdateCategoryRequest(
    int Id,
    Guid UserId,
    string Name,
    string? Description,
    CategoryType Type)
    : UserRequest(UserId: UserId);
