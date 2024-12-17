namespace ExpenseTracker.Application.Requests.Transfer;

public record UpdateTransferRequest(
    int Id,
    string Title,
    string? Notes,
    decimal Amount,
    DateTime Date,
    int CategoryId,
    int WalletId)
    : CreateTransferRequest(
        Title,
        Notes,
        Amount,
        Date,
        CategoryId,
        WalletId);