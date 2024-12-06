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
    private readonly ICurrentUserService _currentUserService;

    public WalletService(
        IMapper mapper,
        IApplicationDbContext context,
        UserManager<IdentityUser<Guid>> userManager,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<List<WalletDto>> GetAsync(QueryParametersBase queryParameters)
    {
        var query = _context.Wallets
            .AsNoTracking()
            .Where(w => w.OwnerId == _currentUserService.GetUserId());

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
        
        var wallet = await GetAndValidateWalletAsync(request.Id);

        var dto = _mapper.Map<WalletDto>(wallet);
        return dto;
    }

    public async Task<WalletDto> CreateAsync(CreateWalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = _mapper.Map<Wallet>(request);
        entity.OwnerId = _currentUserService.GetUserId();

        _context.Wallets.Add(entity);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<WalletDto>(entity);

        return dto;

    }

    public async Task UpdateAsync(UpdateWalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wallet = await GetAndValidateWalletAsync(request.Id);

        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wallet = await GetAndValidateWalletAsync(request.Id);

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();
    }

    private async Task<Wallet> GetAndValidateWalletAsync(int walletId)
    {
        var ownerId = _currentUserService.GetUserId();
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(x => x.Id == walletId && x.OwnerId == ownerId);

        if (wallet is null)
        {
            throw new EntityNotFoundException($"Wallet with id: {walletId} is not found.");
        }

        return wallet;
    }
}
