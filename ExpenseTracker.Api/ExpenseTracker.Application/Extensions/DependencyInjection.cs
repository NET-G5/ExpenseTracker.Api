using ExpenseTracker.Application.Configurations;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Mappings;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Validators.Category;
using ExpenseTracker.Application.Validators.Transfer;
using ExpenseTracker.Application.Validators.Wallet;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(CategoryMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
        services.AddAutoMapper(typeof(TransferMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateTransferValidation>();
        services.AddAutoMapper(typeof(WalletMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateWalletValidation>();
        
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtTokenHandler, JwtTokenHandler>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        AddOptions(services, configuration);

        return services;
    }

    private static void AddOptions(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}
