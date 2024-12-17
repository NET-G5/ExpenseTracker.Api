namespace ExpenseTracker.Application.DTOs;

public record TransferDto(
    int Id,
    string Title,
    string? Notes,
    decimal Amount,
    DateTime Date,
    int CategoryId,
    string CategoryName,
    int WalletId,
    string WalletName);