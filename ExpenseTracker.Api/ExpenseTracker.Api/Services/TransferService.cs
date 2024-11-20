using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services;

internal sealed class TransferService : ITransferService
{
    private readonly ICategoryService _categoryService;
    private readonly static List<Transfer> _transfers = [];

    public TransferService(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Transfer> CreateAsync(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);

        var category = await _categoryService.GetByIdAsync(transfer.CategoryId);
        transfer.Category = category;

        _transfers.Add(transfer);

        return transfer;
    }

    public Task DeleteAsync(int id)
    {
        var transferToDelete = _transfers.Find(x => x.Id == id);

        if (transferToDelete is not null)
        {
            _transfers.Remove(transferToDelete);
        }

        return Task.CompletedTask;
    }

    public Task<List<Transfer>> GetAsync(TransferFilter? queryParameters = null)
    {
        if (queryParameters is null)
        {
            return Task.FromResult(_transfers);
        }

        var query = _transfers.AsQueryable();

        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            query = query.Where(x => x.Name.Contains(queryParameters.Search, StringComparison.InvariantCultureIgnoreCase));
        }

        if (queryParameters.MinAmount.HasValue)
        {
            query = query.Where(x => x.Amount >= queryParameters.MinAmount.Value);
        }

        if (queryParameters.MaxAmount.HasValue)
        {
            query = query.Where(x => x.Amount <= queryParameters.MaxAmount.Value);
        }

        var transfers = query.ToList();

        return Task.FromResult(transfers);
    }

    public Task<Transfer> GetByIdAsync(int id)
    {
        var transfer = _transfers.Find(x => x.Id == id);

        return Task.FromResult(transfer);
    }

    public Task UpdateAsync(Transfer transfer)
    {
        var transferToUpdate = _transfers.Find(x => x.Id == transfer.Id);

        if (transferToUpdate is not null)
        {
            transferToUpdate.Name = transfer.Name;
            transferToUpdate.Amount = transfer.Amount;

        }

        return Task.CompletedTask;
    }
}
