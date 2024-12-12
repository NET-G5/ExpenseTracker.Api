using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Wallet;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<WalletDto>> GetAsync(WalletQueryParameters queryParameters)
    {
        var query = FilterWallets(queryParameters);
        query = SortWallets(query, queryParameters.SortBy);

        var wallets = await query
            .ProjectTo<WalletDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return wallets;
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

    public async Task CreateDefaultForNewUserAsync(IdentityUser<Guid> user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var newWallet = new Wallet
        {
            Name = "Default Wallet",
            Description = "This is default wallet",
            Balance = 0,
            CreatedBy = user.UserName!,
            Owner = user,
        };

        _context.Wallets.Add(newWallet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateWalletRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wallet = _mapper.Map<Wallet>(request);

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
        var currentUserId = _currentUserService.GetUserId();
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.OwnerId == currentUserId);

        if (wallet is null)
        {
            throw new EntityNotFoundException($"Wallet with id: {walletId} is not found.");
        }

        return wallet;
    }

    private IQueryable<Wallet> FilterWallets(WalletQueryParameters queryParameters)
    {
        var currentUserId = _currentUserService.GetUserId();
        var query = _context.Wallets
            .AsNoTracking()
            .Where(x => x.OwnerId == currentUserId);

        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            query = query.Where(x => x.Name.Contains(queryParameters.Search)
                || (x.Description != null && x.Description.Contains(queryParameters.Search)));
        }

        if (queryParameters.MinBalance.HasValue)
        {
            query = query.Where(x => x.Balance >= queryParameters.MinBalance.Value);
        }

        if (queryParameters.MaxBalance.HasValue)
        {
            query = query.Where(x => x.Balance <= queryParameters.MaxBalance.Value);
        }

        return query;
    }

    private static IQueryable<Wallet> SortWallets(IQueryable<Wallet> query, string? sortBy)
       => sortBy switch
       {
           "name_asc" => query.OrderBy(x => x.Name),
           "name_desc" => query.OrderByDescending(x => x.Name),
           "balance_asc" => query.OrderBy(x => x.Balance),
           _ => query.OrderByDescending(x => x.Balance)
       };
}
