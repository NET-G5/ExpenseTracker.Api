using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Application.Interfaces;

public interface IJwtTokenHandler
{
    string GenerateToken(IdentityUser<Guid> user, IEnumerable<string> roles);
    
    
}