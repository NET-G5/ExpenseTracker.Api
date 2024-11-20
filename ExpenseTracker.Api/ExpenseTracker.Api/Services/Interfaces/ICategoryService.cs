using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;

namespace ExpenseTracker.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetAsync(CategoryFilter? filter = null);
    Task<Category> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}
