using ExpenseTracker.Application.Extensions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

internal sealed class NewUserService : INewUserService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IEmailService _emailService;

    public NewUserService(
        UserManager<IdentityUser<Guid>> userManager,
        IApplicationDbContext context,
        IEmailService emailService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task HandlePostRegistrationAsync(RegisterRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);

        if (user is null)
        {
            throw new InvalidOperationException("User was not created correctly.");
        }

        await CreateDefaultCategoryAsync(user);
        await CreateDefaultWalletAsync(user);
        await SendConfirmationEmailAsync(user, request);
    }

    private async Task CreateDefaultCategoryAsync(IdentityUser<Guid> user)
    {
        var income = new Category
        {
            Name = "Default Income Category",
            Description = "This is default income category.",
            CreatedBy = user.UserName!,
            Owner = null!,
            OwnerId = user.Id,
            Type = CategoryType.Income,
        };
        var expense = new Category
        {
            Name = "Default Expense Category",
            Description = "This is default expense category.",
            CreatedBy = user.UserName!,
            Owner = null!,
            OwnerId = user.Id,
            Type = CategoryType.Expense,
        };

        _context.Categories.AddRange(income, expense);
        await _context.SaveChangesAsync();
    }

    private async Task CreateDefaultWalletAsync(IdentityUser<Guid> user)
    {
        var newWallet = new Wallet
        {
            Name = "Default Wallet",
            Description = "This is default wallet",
            Balance = 0,
            CreatedBy = user.UserName!,
            Owner = user,
            OwnerId = user.Id
        };

        _context.Wallets.Add(newWallet);
        await _context.SaveChangesAsync();
    }

    private async Task SendConfirmationEmailAsync(IdentityUser<Guid> user, RegisterRequest request)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var redirectUrl = Helper.GetCallbackUrl(request.ConfirmUrl, token, request.Email);
        var userInfo = new UserInfo(request.Browser ?? "Unknown browser", request.OS ?? "Unknown operating system");
        var emailMessage = new EmailMessage(user.Email!, user.UserName!, "Email confirmation", redirectUrl);

        _emailService.SendEmailConfirmation(emailMessage, userInfo);
    }

}
