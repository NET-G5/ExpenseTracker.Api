using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.DTOs;

public record WalletDto(
    int Id,
    string Name,
    string? Description,
    decimal Balance);