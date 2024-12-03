using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Requests.Wallet;

public record CreateWalletRequest(
    string Name,
    string? Description,
    decimal Balance,
    Guid OwnerId,
    IdentityUser<Guid> Owner,
    List<Domain.Entities.Transfer> Transfers);