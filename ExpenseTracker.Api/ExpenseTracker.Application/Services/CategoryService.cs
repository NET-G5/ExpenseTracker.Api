using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.Requests.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContext _context;

    public CategoryService(IMapper mapper, IApplicationDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var entity = _mapper.Map<Category>(request);

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<CategoryDto>(entity);

        return dto;
    }

    public async Task DeleteAsync(CategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var categoryToDelete = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

        if (categoryToDelete is null)
        {
            throw new EntityNotFoundException($"Category with id: {request.CategoryId} is not found.");
        }

        if (categoryToDelete.OwnerId != request.UserId)
        {
            throw new ApplicationException($"Current user has no right to delete category with id: {request.CategoryId}.");
        }

        _context.Categories.Remove(categoryToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<List<CategoryDto>> GetAsync(UserRequest request, QueryParametersBase queryParameters)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(x => x.OwnerId == request.UserId);

        if (!string.IsNullOrEmpty(queryParameters.Search))
        {
            query = query.Where(x => x.Name.Contains(queryParameters.Search)
                || (x.Description != null && x.Description.Contains(queryParameters.Search)));
        }

        var categories = await query.ToArrayAsync();
        var dtos = _mapper.Map<List<CategoryDto>>(categories);

        return dtos;
    }

    public async Task<CategoryDto> GetByIdAsync(CategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId && x.OwnerId == request.UserId);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with id: {request.CategoryId} is not found.");
        }

        var dto = _mapper.Map<CategoryDto>(category);

        return dto;
    }

    public async Task UpdateAsync(UpdateCategoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = _mapper.Map<Category>(request);

        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
    }
}
