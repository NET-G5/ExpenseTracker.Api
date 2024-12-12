using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.Requests.Transfer;

namespace ExpenseTracker.Application.Interfaces;

public interface ITransferService
{
    Task<PaginatedResponse<TransferDto>> GetAsync(TransferQueryParameters queryParameters);
    Task<TransferDto> GetByIdAsync(TransferRequest request);
    Task<List<TransferDto>> GetByCategoryAsync(CategoryRequest request);
    Task<TransferDto> CreateAsync(CreateTransferRequest request);
    Task UpdateAsync(UpdateTransferRequest request);
    Task DeleteAsync(TransferRequest request);
}