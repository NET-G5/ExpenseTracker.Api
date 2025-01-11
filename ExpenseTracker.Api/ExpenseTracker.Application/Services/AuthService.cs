using ExpenseTracker.Application.Extensions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Domain.Exceptions;
using Hangfire;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Services;

internal sealed class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly INewUserService _newUserService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager,
        IJwtTokenHandler jwtTokenHandler,
        IEmailService emailService,
        IBackgroundJobClient backgroundJobClient,
        INewUserService newUserService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _jwtTokenHandler = jwtTokenHandler ?? throw new ArgumentNullException(nameof(jwtTokenHandler));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
        _newUserService = newUserService;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null || !user.EmailConfirmed || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new InvalidLoginRequestException("Invalid username or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenHandler.GenerateToken(user, roles);

        return token;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingUser = await _userManager.FindByNameAsync(request.UserName);

        if (existingUser is not null)
        {
            throw new UserNameAlreadyTakenException($"Username: {request.UserName} is already taken.");
        }

        var newUser = new IdentityUser<Guid>
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
        };

        var createResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"User creation failed: {errors}");
        }

        _backgroundJobClient.Enqueue(() => _newUserService.HandlePostRegistrationAsync(request));
    }

    public async Task ConfirmEmailAsync(EmailConfirmationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await GetAndValidateUserAsync(request.Email);

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Email confirmation failed. {errors}");
        }

        await SendWelcomeEmailAsync(user);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await GetAndValidateUserAsync(request.Email);

        await SendPasswordResetEmailAsync(user, request);
    }

    public async Task ConfirmResetPasswordAsync(ConfirmResetPasswordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await GetAndValidateUserAsync(request.Email);

        if (!user.EmailConfirmed)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Email confirmation failed. {errors}");
        }
    }

    private async Task SendWelcomeEmailAsync(IdentityUser<Guid> user)
    {
        var emailMessage = new EmailMessage(user.Email!, user.UserName!, "Welcome to Expense Tracker!", null);

        _backgroundJobClient.Enqueue(() => _emailService.SendWelcome(emailMessage));
    }

    private async Task SendPasswordResetEmailAsync(IdentityUser<Guid> user, ResetPasswordRequest request)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var redirectUrl = Helper.GetCallbackUrl(request.RedirectUrl, token, request.Email);
        var emailMessage = new EmailMessage(user.Email!, user.UserName!, "Password Reset", redirectUrl);
        var userInfo = new UserInfo(request.Browser, request.OS);

        _backgroundJobClient.Enqueue(() => _emailService.SendResetPassword(emailMessage, userInfo));
    }

    private async Task<IdentityUser<Guid>> GetAndValidateUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            throw new EntityNotFoundException($"User with email: {email} is not found.");
        }

        return user;
    }
}