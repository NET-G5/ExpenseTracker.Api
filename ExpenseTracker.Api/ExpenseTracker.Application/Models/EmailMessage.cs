namespace ExpenseTracker.Application.Models;

public sealed record EmailMessage(
    string To,
    string Username,
    string Subject,
    string? FallbackUrl);