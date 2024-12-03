using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Transfer;

namespace ExpenseTracker.Application.Interfaces;

public interface ITranferService
{
    Task<List<TransferDto>> GetAsync(QueryParametersBase queryParameters);
    Task<TransferDto> GetByIdAsync(TransferRequest request);
    Task<TransferDto> CreateAsync(CreateTranferRequest request);
    Task UpdateAsync(UpdateTranferRequest request);
    Task DeleteAsync(TransferRequest request);
}