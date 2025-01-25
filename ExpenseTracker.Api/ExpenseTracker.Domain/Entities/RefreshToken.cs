using ExpenseTracker.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Domain.Entities;

public class RefreshToken : EntityBase
{
    public required string Token { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }

    public required Guid UserId { get; set; }
    public required IdentityUser<Guid> User { get; set; }
}
