using System.Security.Claims;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Application.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    
    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User 
                   ?? throw new InvalidOperationException("Current HTTP context does not contain a user.");

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? throw new InvalidOperationException("User does not have a NameIdentifier claim.");

        if (Guid.TryParse(userId, out var result))
        {
            return result;
        }
        
        throw new InvalidOperationException($"Invalid user ID format: {userId}");
    }

    public string GetUserName()
    {
        var user = _httpContextAccessor.HttpContext?.User
            ?? throw new InvalidOperationException($"Unable to get user info from HttpContext.");

        var userName = user.FindFirst(ClaimTypes.Name)?.Value
            ?? throw new InvalidOperationException("User does not have Name claim.");

        return userName;
    }
}