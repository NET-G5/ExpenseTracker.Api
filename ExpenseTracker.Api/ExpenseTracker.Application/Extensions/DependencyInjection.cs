using ExpenseTracker.Application.BackgroundJobs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Mappings;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Validators.Category;
using FluentValidation;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ExpenseTracker.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(CategoryMappings).Assembly);
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
        services.AddFluentValidationAutoValidation();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMonthlyReportService, MonthlyReportService>();
        services.AddScoped<INewUserService, NewUserService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddSingleton<IJwtTokenHandler, JwtTokenHandler>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddHostedService<MonthlyReportBackgroundService>();

        AddBackgroundJobs(services, configuration);

        return services;
    }

    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(options =>
        {
            options.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddHangfireServer();
    }
}
