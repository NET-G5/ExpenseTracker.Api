using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;

namespace ExpenseTracker.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAsync(CategoryQueryParameters queryParameters);
    Task<CategoryDto> GetByIdAsync(CategoryRequest request);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(UpdateCategoryRequest request);
    Task DeleteAsync(CategoryRequest request);
}
