using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Mappings;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Validators.Category;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CategoryMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
