using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using AutoMapper.QueryableExtensions;

namespace ExpenseTracker.Application.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CategoryService(
        IMapper mapper,
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<List<CategoryDto>> GetAsync(QueryParametersBase queryParameters)
    {
        ArgumentNullException.ThrowIfNull(queryParameters);

        var query = _context.Categories
            .AsNoTracking()
            .Where(x => x.OwnerId == _currentUserService.GetUserId());

        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            query = query.Where(x => x.Name.Contains(queryParameters.Search)
                || (x.Description != null && x.Description.Contains(queryParameters.Search)));
        }

        query = ApplySort(query, queryParameters.SortBy);

        var categories = await query
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return categories;
    }

    public async Task<CategoryDto> GetByIdAsync(CategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await GetAndValidateCategoryAsync(request.Id);
        var dto = _mapper.Map<CategoryDto>(category);

        return dto;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = _mapper.Map<Category>(request);
        entity.OwnerId = _currentUserService.GetUserId();

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<CategoryDto>(entity);

        return dto;
    }

    public async Task UpdateAsync(UpdateCategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await GetAndValidateCategoryAsync(request.Id);

        _mapper.Map(request, category);

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await GetAndValidateCategoryAsync(request.Id);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    private async Task<Category> GetAndValidateCategoryAsync(int categoryId)
    {
        var ownerId = _currentUserService.GetUserId();
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == categoryId && x.OwnerId == _currentUserService.GetUserId());

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with id: {categoryId} is not found.");
        }

        return category;
    }

    private static IQueryable<Category> ApplySort(IQueryable<Category> query, string? sortBy)
        => sortBy switch
        {
            "name_desc" => query.OrderByDescending(x => x.Name),
            "description_asc" => query.OrderBy(x => x.Description),
            "description_desc" => query.OrderByDescending(x => x.Description),
            _ => query.OrderBy(x => x.Name)
        };
}
