using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Extensions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.Requests.Transfer;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

internal sealed class TransferService : ITransferService
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

    public async Task<PaginatedResponse<TransferDto>> GetAsync(TransferQueryParameters queryParameters)
    {
        var query = FilterTransfers(queryParameters);
        query = SortTransfers(query, queryParameters.SortBy);

        var transfers = await query
            .ProjectTo<TransferDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(queryParameters.PageSize, queryParameters.PageNumber);

        return new PaginatedResponse<TransferDto>(transfers, transfers.Metadata);
    }

    public async Task<List<TransferDto>> GetByCategoryAsync(CategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentUserId = _currentUserService.GetUserId();
        var transfers = await _context.Transfers
            .Where(x => x.CategoryId == request.Id && x.Category.OwnerId == currentUserId)
            .ProjectTo<TransferDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return transfers;
    }

    public async Task<TransferDto> GetByIdAsync(TransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentUserId = _currentUserService.GetUserId();
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.Wallet.OwnerId == currentUserId);

        if (transfer is null)
        {
            throw new EntityNotFoundException($"Transfer with id: {request.Id} is not found.");
        }

        var dto = _mapper.Map<TransferDto>(transfer);

        return dto;
    }

    public async Task<TransferDto> CreateAsync(CreateTransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transfer = _mapper.Map<Transfer>(request);

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<TransferDto>(transfer);
        return dto;
    }

    public async Task UpdateAsync(UpdateTransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var transfer = _mapper.Map<Transfer>(request);

        _context.Transfers.Update(transfer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TransferRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currentUserId = _currentUserService.GetUserId();
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.Wallet.OwnerId == currentUserId);

        if (transfer is null)
        {
            throw new EntityNotFoundException($"Transfer with id: {request.Id} is not found.");
        }

        _context.Transfers.Remove(transfer);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Transfer> FilterTransfers(TransferQueryParameters queryParameters)
    {
        ArgumentNullException.ThrowIfNull(queryParameters);

        var currentUserId = _currentUserService.GetUserId();
        var query = _context.Transfers
            .AsNoTracking()
            .Where(x => x.Wallet.OwnerId == currentUserId);

        if (queryParameters.MinAmount.HasValue)
        {
            query = query.Where(x => x.Amount >= queryParameters.MinAmount.Value);
        }

        if (queryParameters.MaxAmount.HasValue)
        {
            query = query.Where(x => x.Amount <= queryParameters.MaxAmount.Value);
        }

        if (queryParameters.MinDate.HasValue)
        {
            query = query.Where(x => DateOnly.FromDateTime(x.Date.Date) >= queryParameters.MinDate.Value);
        }

        if (queryParameters.MaxDate.HasValue)
        {
            query = query.Where(x => DateOnly.FromDateTime(x.Date.Date) <= queryParameters.MaxDate.Value);
        }

        if (queryParameters.Type.HasValue)
        {
            query = query.Where(x => x.Category.Type == queryParameters.Type.Value);
        }

        return query;
    }

    private static IQueryable<Transfer> SortTransfers(IQueryable<Transfer> query, string sortBy)
        => sortBy switch
        {
            "title_asc" => query.OrderBy(x => x.Title),
            "title_desc" => query.OrderByDescending(x => x.Title),
            "amount_asc" => query.OrderBy(x => x.Amount),
            "amount_desc" => query.OrderByDescending(x => x.Amount),
            "date_asc" => query.OrderBy(x => x.Date),
            _ => query.OrderByDescending(x => x.Date)
        };
}
