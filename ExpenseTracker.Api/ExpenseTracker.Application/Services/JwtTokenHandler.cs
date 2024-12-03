using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Application.Services;

internal sealed class JwtTokenHandler : IJwtTokenHandler
{
    public string GenerateToken(IdentityUser<Guid> user, IEnumerable<string> roles)
    {
        var claims = GetClaims(user, roles);
        var signingKey = GetClaimingKey();
        var securityToken = new JwtSecurityToken(
            issuer: "expense-tracker-api",
            audience: "expense-tracker",
            claims: claims,
            signingCredentials: signingKey,
            expires: DateTime.UtcNow.AddHours(5));

        var  token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return token;
    }

    private static List<Claim> GetClaims(IdentityUser<Guid> user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
    
    private static SigningCredentials GetClaimingKey()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("cacd9786-4fea-413c-94af-abe7b6ee54f4"));
        var signingKey = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        return signingKey;
    }
}