using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAsync(CategoryQueryParameters queryParameters);
    Task<CategoryDto> GetByIdAsync(CategoryRequest request);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task CreateDefaultsForNewUserAsync(IdentityUser<Guid> user);
    Task UpdateAsync(UpdateCategoryRequest request);
    Task DeleteAsync(CategoryRequest request);
}
