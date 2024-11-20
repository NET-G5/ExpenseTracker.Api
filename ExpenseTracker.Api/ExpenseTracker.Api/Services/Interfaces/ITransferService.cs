using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ITransferService
{
    Task<List<Transfer>> GetAsync(TransferFilter? queryParameters = null);
    Task<Transfer> GetByIdAsync(int id);
    Task<Transfer> CreateAsync(Transfer transfer);
    Task UpdateAsync(Transfer transfer);
    Task DeleteAsync(int id);
}
