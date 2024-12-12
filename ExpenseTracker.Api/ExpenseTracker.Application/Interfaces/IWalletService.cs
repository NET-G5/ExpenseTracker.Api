using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Wallet;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Interfaces;

public interface IWalletService
{
    Task<List<WalletDto>> GetAsync(WalletQueryParameters queryParameters);
    Task<WalletDto> GetByIdAsync(WalletRequest request);
    Task<WalletDto> CreateAsync(CreateWalletRequest request);
    Task CreateDefaultForNewUserAsync(IdentityUser<Guid> user);
    Task UpdateAsync(UpdateWalletRequest request);
    Task DeleteAsync(WalletRequest request);
}