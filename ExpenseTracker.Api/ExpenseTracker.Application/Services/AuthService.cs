using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Requests.Auth;
using ExpenseTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Services;

internal sealed class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly ICategoryService _categoryService;
    private readonly IWalletService _walletService;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager, 
        IJwtTokenHandler jwtTokenHandler,
        ICategoryService categoryService,
        IWalletService walletService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _jwtTokenHandler = jwtTokenHandler ?? throw new ArgumentNullException(nameof(jwtTokenHandler));
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
    }
    
    public async Task<string> LoginAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user =  await _userManager.FindByNameAsync(request.UserName);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
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
            Email = request.Email
        };

        var createResult = await _userManager.CreateAsync(newUser, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"User creation failed: {errors}");
        }

        await _categoryService.CreateDefaultsForNewUserAsync(newUser);
        await _walletService.CreateDefaultForNewUserAsync(newUser);
    }

}