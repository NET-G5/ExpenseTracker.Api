using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Transfer;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services;

internal sealed class TransferService : ITranferService
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TransferService(
        IMapper mapper,
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }
    public async Task<List<TransferDto>> GetAsync( QueryParametersBase queryParameters)
    {
        var query = _context.Transfers
            .AsNoTracking()
            .Where(x => x.Category.OwnerId == _currentUserService.GetUserId());

        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            query = query.Where(x => x.Title.Contains(queryParameters.Search)
                || (x.Notes != null && x.Notes.Contains(queryParameters.Search)));
        }

        var transfers = await query.ToArrayAsync();
        var dtos = _mapper.Map<List<TransferDto>>(transfers);
        return dtos;
    }


    public async Task<TransferDto> GetByIdAsync(TransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transfer = await GetAndValidateTransferAsync(request.Id);

        var dto = _mapper.Map<TransferDto>(transfer);
        return dto;
    }

    public async Task<TransferDto> CreateAsync(CreateTransferRequest request)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId && x.OwnerId == _currentUserService.GetUserId());
        
        if (category is null)
        {
            throw new EntityNotFoundException($"Category with id: {request.CategoryId} is not found.");
        }

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.OwnerId == _currentUserService.GetUserId());
        
        if (wallet is null)
        {
            throw new EntityNotFoundException($"Wallet with id: {request.WalletId} is not found.");
        }

        var transfer = _mapper.Map<Transfer>(request);
        transfer.Date = DateTime.UtcNow;

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<TransferDto>(transfer);
        return dto;
    }

    public async Task UpdateAsync(UpdateTransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

      var transfer = await GetAndValidateTransferAsync(request.Id);

        _context.Transfers.Update(transfer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

      var transfer = await GetAndValidateTransferAsync(request.Id); 

        _context.Transfers.Remove(transfer);
        await _context.SaveChangesAsync();
    }

    private async Task<Transfer> GetAndValidateTransferAsync(int transferId)
    {
        var ownerId = _currentUserService.GetUserId();
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == transferId && x.Category.OwnerId == ownerId);

        if (transfer is null)
        {
            throw new EntityNotFoundException($"Transfer with id: {transferId} is not found.");
        }

        return transfer;
    }



}
