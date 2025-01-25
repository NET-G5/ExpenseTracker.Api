using ExpenseTracker.Application.Configurations;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Application.Responses.Auth;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Exceptions;
using ExpenseTracker.Domain.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Application.Services;

internal sealed class AuthService : IAuthService
{
    private readonly TokenSettings _tokenSettings;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly ITokenHandler _jwtTokenHandler;
    private readonly INewUserService _newUserService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;
    private readonly IApplicationDbContext _context;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager,
        ITokenHandler jwtTokenHandler,
        IEmailService emailService,
        IBackgroundJobClient backgroundJobClient,
        INewUserService newUserService,
        ILogger<AuthService> logger,
        IApplicationDbContext context,
        IOptions<TokenSettings> tokenSettings)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _jwtTokenHandler = jwtTokenHandler ?? throw new ArgumentNullException(nameof(jwtTokenHandler));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
        _newUserService = newUserService ?? throw new ArgumentNullException(nameof(newUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tokenSettings = tokenSettings?.Value ?? throw new ArgumentNullException(nameof(tokenSettings));
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user is null || !user.EmailConfirmed || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogWarning("Invalid login attempt for username {UserName}", request.UserName);
            throw new InvalidLoginRequestException("Invalid username or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenHandler.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenHandler.GenerateRefreshToken();
        await SaveRefreshTokenAsync(user, refreshToken);


        return new LoginResponse(accessToken, refreshToken);
    }

    public async Task<RefreshTokenResponse> RefreshAsync(RefreshTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (token is null || token.IsRevoked)
        {
            throw new InvalidLoginRequestException("TODO: Change exception type");
        }

        if (token.ExpiresAtUtc < DateTime.UtcNow)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();

            throw new InvalidLoginRequestException("TODO: Change exception type");
        }

        var roles = await _userManager.GetRolesAsync(token.User);
        var accessToken = _jwtTokenHandler.GenerateAccessToken(token.User, roles);

        return new RefreshTokenResponse(accessToken);
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingUser = await _userManager.FindByNameAsync(request.UserName);

        if (existingUser is not null)
        {
            _logger.LogWarning("Registration attempt with existing username {UserName}", request.UserName);
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
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == user.Id && !x.IsRevoked);

        if (refreshToken is not null)
        {
            refreshToken.IsRevoked = true;
        }

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

        _backgroundJobClient.Enqueue("email_welcome", () => _emailService.SendWelcome(emailMessage));
    }

    private async Task SendPasswordResetEmailAsync(IdentityUser<Guid> user, ResetPasswordRequest request)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var redirectUrl = $"{request.RedirectUrl}?token={token}&email={request.Email}";

        var emailMessage = new EmailMessage(user.Email!, user.UserName!, "Password Reset", redirectUrl);
        var userInfo = new UserInfo(request.Browser, request.OS);

        _backgroundJobClient.Enqueue("email_reset-password", () => _emailService.SendResetPassword(emailMessage, userInfo));
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

    private async Task SaveRefreshTokenAsync(IdentityUser<Guid> user, string refreshToken)
    {
        var tokenEntity = new RefreshToken
        {
            Token = refreshToken,
            IsRevoked = false,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokenSettings.RefreshExpiresInDays),
            UserId = user.Id,
            User = user,
        };
        _context.RefreshTokens.Add(tokenEntity);
        await _context.SaveChangesAsync();
    }
}