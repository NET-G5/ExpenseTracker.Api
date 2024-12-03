using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.Application.Configurations;
using ExpenseTracker.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Application.Services;

internal sealed class JwtTokenHandler : IJwtTokenHandler
{
    private readonly JwtOptions _options;

    public JwtTokenHandler(IOptions<JwtOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string GenerateToken(IdentityUser<Guid> user, IEnumerable<string> roles)
    {
        var claims = GetClaims(user, roles);
        var signingKey = GetClaimingKey();
        var securityToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            signingCredentials: signingKey,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresInHours));

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
    
    private SigningCredentials GetClaimingKey()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var signingKey = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        return signingKey;
    }
}