using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

internal sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable(nameof(Wallet));
        builder.HasKey(w => w.Id);

        builder
            .HasOne(w => w.Owner)
            .WithMany()
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .HasMany(w => w.Transfers)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .Property(w => w.Name)
            .HasMaxLength(Constants.DEFAULT_STRING_LENGTH)
            .IsRequired();

        builder
            .Property(w => w.Description)
            .HasMaxLength(Constants.MAX_STRING_LENGTH)
            .IsRequired(false);

        builder
            .Property(w => w.Balance)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}
