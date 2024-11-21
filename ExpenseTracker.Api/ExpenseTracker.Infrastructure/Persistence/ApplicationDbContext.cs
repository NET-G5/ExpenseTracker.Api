using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ExpenseTracker.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Transfer> Transfers { get; set; }
    public virtual DbSet<Wallet> Wallets { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.AddInterceptors(new AuditInterceptor());

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

        #region Identity

        modelBuilder.Entity<IdentityUser<Guid>>(e =>
        {
            e.ToTable("User");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(e =>
        {
            e.ToTable("UserClaim");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(e =>
        {
            e.ToTable("UserLogin");
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(e =>
        {
            e.ToTable("UserToken");
        });

        modelBuilder.Entity<IdentityRole<Guid>>(e =>
        {
            e.ToTable("Role");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(e =>
        {
            e.ToTable("RoleClaim");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(e =>
        {
            e.ToTable("UserRole");
        });

        #endregion
    }

    public Task<int> SaveChangesAsync() => SaveChangesAsync();
}
