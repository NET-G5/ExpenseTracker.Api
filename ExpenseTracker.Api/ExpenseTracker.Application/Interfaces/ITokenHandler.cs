using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Interfaces;

public interface ITokenHandler
{
    string GenerateAccessToken(IdentityUser<Guid> user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}