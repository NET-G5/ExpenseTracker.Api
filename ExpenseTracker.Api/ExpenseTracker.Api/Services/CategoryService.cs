using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;
using ExpenseTracker.Api.Services.Interfaces;

namespace ExpenseTracker.Api.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly static List<Category> _categories = [];

    public Task<Category> CreateAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _categories.Add(category);

        return Task.FromResult(category);
    }

    public Task DeleteAsync(int id)
    {
        var categoryToDelete = _categories.Find(x => x.Id == id);

        if (categoryToDelete is not null)
        {
            _categories.Remove(categoryToDelete);
        }

        return Task.CompletedTask;
    }

    public Task<List<Category>> GetAsync(CategoryFilter? filter = null)
    {
        if (filter is null)
        {
            return Task.FromResult(_categories);
        }

        var categories = _categories
            .Where(x => x.Name.Contains(filter.Name))
            .Where(x => x.Type == filter.Type)
            .ToList();

        return Task.FromResult(categories);
    }

    public Task<Category> GetByIdAsync(int id)
        => Task.FromResult(_categories.Find(x => x.Id == id));

    public Task UpdateAsync(Category category)
    {
        var categoryToUpdate = _categories.Find(x => x.Id == category.Id);

        if (categoryToUpdate is not null)
        {
            categoryToUpdate.Name = category.Name;
            categoryToUpdate.Type = category.Type;
        }

        return Task.CompletedTask;
    }
}
