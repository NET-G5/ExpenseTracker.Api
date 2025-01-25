using ExpenseTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Transfer> Transfers { get; set; }
    DbSet<Wallet> Wallets { get; set; }
    DbSet<IdentityUser<Guid>> Users { get; set; }
    DbSet<IdentityRole<Guid>> Roles { get; set; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
