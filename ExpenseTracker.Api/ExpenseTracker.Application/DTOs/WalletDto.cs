using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.DTOs;

public record WalletDto(
    int Id,
    string Name,
    string? Description,
    decimal Balance,
    Guid OwnerId,
    IdentityUser<Guid> Owner,
    List<Domain.Entities.Transfer> Transfers);