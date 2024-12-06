using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Requests.Wallet;

public record UpdateWalletRequest(
    int Id,
    string Name,
    string? Description,
    decimal Balance)
    : CreateWalletRequest(
        Name,
        Description,
        Balance);