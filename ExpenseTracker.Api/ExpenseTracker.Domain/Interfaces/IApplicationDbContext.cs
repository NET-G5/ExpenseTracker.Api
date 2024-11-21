using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Transfer> Transfers { get; set; }
    DbSet<Wallet> Wallets { get; set; }

    Task<int> SaveChangesAsync();
}
