namespace ExpenseTracker.Application.Requests.Transfer;

public record UpdateTranferRequest(
    int Id,
    string Title,
    string? Notes,
    decimal Amount,
    DateTime Date,
    int CategoryId,
    int WalletId)
    : CreateTranferRequest(
        Title,
        Notes,
        Amount,
        Date,
        CategoryId,
        WalletId);