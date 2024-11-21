using AutoMapper;
using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Mappings;

internal sealed class CategoryMappings : Profile
{
    public CategoryMappings()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<UpdateCategoryRequest, Category>();
    }
}
