namespace ExpenseTracker.Application.Models;

public sealed record PaginatedResponse<T>(
    List<T> Data,
    PaginationMetadata PaginationMetadata);
