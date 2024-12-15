using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpenseTracker.Application.BackgroundJobs;

internal sealed class MonthlyReportBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MonthlyReportBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            if (now.Day == 1)
            {
                await SendMonthlyReportAsync();
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task SendMonthlyReportAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var users = await context.Users.ToListAsync();

        foreach (var user in users)
        {
            var totalAmount = await CalculateTotalAmountAsync(user, context);

            emailService.SendEmailConfirmation(new EmailMessage(user.Email!, user.UserName + " " + totalAmount, "Monthly report", null), null);
        }
    }

    private async Task<decimal> CalculateTotalAmountAsync(IdentityUser<Guid> user, IApplicationDbContext context)
    {
        var sum = await context.Transfers
            .Where(x => x.Category.OwnerId == user.Id)
            .SumAsync(x => x.Amount);

        return sum;

        // Reports
    }
}
