using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.DTOs;

public sealed record CategoryDto(
    int Id,
    string Name,
    string? Description,
    CategoryType Type);
