using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Mappings;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Validators.Category;
using ExpenseTracker.Application.Validators.Transfer;
using ExpenseTracker.Application.Validators.Wallet;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CategoryMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
        services.AddAutoMapper(typeof(TransferMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateTransferValidation>();
        services.AddAutoMapper(typeof(WalletMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateWalletValidation>();
        
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITranferService, TransferService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtTokenHandler, JwtTokenHandler>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
