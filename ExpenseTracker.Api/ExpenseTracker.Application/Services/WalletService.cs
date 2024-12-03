using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services;

public class WalletService : IWalletService
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public WalletService(
        IMapper mapper,
        IApplicationDbContext context,
        UserManager<IdentityUser<Guid>> userManager,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<List<WalletDto>> GetAsync(QueryParametersBase queryParameters)
    {
        var query = _context.Wallets
            .AsNoTracking()
            .Where(w => w.OwnerId == _currentUserService.GetCurrentUserId());

        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            query = query.Where(w => w.Name.Contains(queryParameters.Search)
                || (w.Description != null && w.Description.Contains(queryParameters.Search)));
        }

        var wallets = await query.ToListAsync();
        var walletDtos = _mapper.Map<List<WalletDto>>(wallets);

        return walletDtos;
    }

    public async Task<WalletDto> GetByIdAsync(WalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.OwnerId == _currentUserService.GetCurrentUserId());

        if (wallet is null)
        {
            throw new EntityNotFoundException($"Wallet with id: {request.WalletId} is not found.");
        }

        var dto = _mapper.Map<WalletDto>(wallet);
        return dto;
    }

    public async Task<WalletDto> CreateAsync(CreateWalletRequest request)
    {
        var owner = await _userManager.FindByIdAsync(_currentUserService.GetCurrentUserId().ToString());

        if (owner is null)
        {
            throw new EntityNotFoundException($"User is not found.");
        }

        var wallet = _mapper.Map<Wallet>(request);
        wallet.OwnerId = _currentUserService.GetCurrentUserId();

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<WalletDto>(wallet);
        return dto;
    }

    public async Task UpdateAsync(UpdateWalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.OwnerId == _currentUserService.GetCurrentUserId());

        if (wallet is null)
        {
            throw new EntityNotFoundException($"Wallet with id: {request.Id} is not found.");
        }

        wallet.Name = request.Name ?? wallet.Name;
        wallet.Description = request.Description ?? wallet.Description;
        wallet.Balance = request.Balance != default ? request.Balance : wallet.Balance;

        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == request.WalletId && w.OwnerId == _currentUserService.GetCurrentUserId());

        if (wallet is null)
        {
            throw new EntityNotFoundException($"Wallet with id: {request.WalletId} is not found.");
        }

        if (wallet.Transfers.Any())
        {
            throw new ApplicationException($"Cannot delete wallet with transfers associated.");
        }

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();
    }
}
