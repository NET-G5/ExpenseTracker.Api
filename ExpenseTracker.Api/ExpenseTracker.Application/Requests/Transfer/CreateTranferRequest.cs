namespace ExpenseTracker.Application.Requests.Transfer;

public record CreateTransferRequest(
    string Title,
    string? Notes,
    decimal Amount,
    DateTime Date,
    int CategoryId,
    int WalletId);