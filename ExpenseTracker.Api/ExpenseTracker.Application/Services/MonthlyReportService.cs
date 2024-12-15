using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Sms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

internal sealed class MonthlyReportService : IMonthlyReportService
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;

    public MonthlyReportService(
        IApplicationDbContext context,
        IEmailService emailService,
        ISmsService smsService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
    }

    public async Task SendMonthlyReportToAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();

        foreach (var user in users)
        {
            var totalAmount = await CalculateTotalAmountAsync(user);

            _emailService.SendEmailConfirmation(new EmailMessage(user.Email!, user.UserName + " " + totalAmount, "Monthly report", null), null);

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var message = new SmsMessage(user.PhoneNumber, "Bu Eskiz dan test");
                await _smsService.SendMessage(message);
            }
        }
    }

    private async Task<decimal> CalculateTotalAmountAsync(IdentityUser<Guid> user)
    {
        var sum = await _context.Transfers
            .Where(x => x.Category.OwnerId == user.Id)
            .SumAsync(x => x.Amount);

        return sum;

        // Reports
    }
}
