using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Requests.Wallet;

public record UpdateWalletRequest(
    int Id,
    string Name,
    string? Description,
    decimal Balance,
    Guid OwnerId,
    IdentityUser<Guid> Owner,
    List<Domain.Entities.Transfer> Transfers)
    : CreateWalletRequest(
        Name,
        Description,
        Balance,
        OwnerId,
        Owner,
        Transfers);