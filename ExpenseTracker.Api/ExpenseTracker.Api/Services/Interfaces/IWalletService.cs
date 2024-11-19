using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface IWalletService
{
    Task<List<Wallet>> GetAsync(WalletFilter? filter = null);
    Task<Wallet> GetByIdAsync(int id);
    Task<Wallet> CreateAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task DeleteAsync(int id);
}
