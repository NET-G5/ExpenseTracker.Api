using ExpenseTracker.Application.Configurations;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Email;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Interceptors;
using ExpenseTracker.Infrastructure.Sms;
using FluentEmail.MailKitSmtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
        });

        AddIdentity(services);
        AddEmail(services, configuration);
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }

    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(12);
        });
    }

    private static void AddEmail(IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(EmailOptions.SectionName);
        var emailOptions = section.Get<EmailOptions>();

        if (emailOptions is null)
        {
            throw new InvalidOperationException("Cannot setup email without configuration values.");
        }

        var smtpOptions = new SmtpClientOptions
        {
            Server = emailOptions.Server,
            Port = emailOptions.Port,
            User = emailOptions.FromEmail,
            Password = emailOptions.Password,
            UseSsl = true,
            RequiresAuthentication = true,
        };

        services
            .AddFluentEmail(emailOptions.FromEmail, emailOptions.FromName)
            .AddRazorRenderer()
            .AddMailKitSender(smtpOptions);

        services.AddScoped<IEmailService, EmailService>();
    }
}