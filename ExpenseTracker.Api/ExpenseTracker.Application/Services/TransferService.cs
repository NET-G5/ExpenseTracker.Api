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

    public async Task<TransferDto> CreateAsync(CreateTranferRequest request)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId && x.OwnerId == _currentUserService.GetCurrentUserId());
        
        if (category is null)
        {
            throw new EntityNotFoundException($"Category with id: {request.CategoryId} is not found.");
        }

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(x => x.Id == request.WalletId && x.OwnerId == _currentUserService.GetCurrentUserId());
        
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

    public async Task DeleteAsync(TransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transferToDelete = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == request.TransferId && x.Wallet.OwnerId == _currentUserService.GetCurrentUserId());

        if (transferToDelete is null)
        {
            throw new EntityNotFoundException($"Transfer with id: {request.TransferId} is not found.");
        }

        if (transferToDelete.Wallet.OwnerId != _currentUserService.GetCurrentUserId())
        {
            throw new ApplicationException($"Current user has no right to delete transfer with id: {request.TransferId}.");
        }

        _context.Transfers.Remove(transferToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TransferDto>> GetAsync(QueryParametersBase queryParameters)
    {
        var query = _context.Transfers
            .AsNoTracking()
            .Where(x => x.Wallet.OwnerId == _currentUserService.GetCurrentUserId());

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

        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == request.TransferId && x.Wallet.OwnerId == _currentUserService.GetCurrentUserId());

        if (transfer is null)
        {
            throw new EntityNotFoundException($"Transfer with id: {request.TransferId} is not found.");
        }

        var dto = _mapper.Map<TransferDto>(transfer);
        return dto;
    }

    public async Task UpdateAsync(UpdateTranferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.Wallet.OwnerId == _currentUserService.GetCurrentUserId());

        if (transfer is null)
        {
            throw new EntityNotFoundException($"Transfer with id: {request.Id} is not found.");
        }

        transfer.Title = request.Title ?? transfer.Title;
        transfer.Notes = request.Notes ?? transfer.Notes;
        transfer.Amount = request.Amount != default ? request.Amount : transfer.Amount;
        transfer.CategoryId = request.CategoryId != default ? request.CategoryId : transfer.CategoryId;

        _context.Transfers.Update(transfer);
        await _context.SaveChangesAsync();
    }
}
