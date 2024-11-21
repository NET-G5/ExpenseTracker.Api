using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.Requests.Common;

namespace ExpenseTracker.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAsync(UserRequest request, QueryParametersBase queryParameters);
    Task<CategoryDto> GetByIdAsync(CategoryRequest request);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(UpdateCategoryRequest request);
    Task DeleteAsync(CategoryRequest request);
}
