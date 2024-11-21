using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

internal sealed class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.ToTable(nameof(Transfer));
        builder.HasKey(t => t.Id);

        builder
            .Navigation(t => t.Category)
            .AutoInclude();

        builder
            .HasOne(t => t.Category)
            .WithMany(c => c.Transfers)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .HasOne(t => t.Wallet)
            .WithMany(w => w.Transfers)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .Property(t => t.Notes)
            .HasMaxLength(Constants.DEFAULT_STRING_LENGTH)
            .IsRequired(false);

        builder
            .Property(t => t.Amount)
            .HasPrecision(13, 2)
            .IsRequired();

        builder
            .Property(t => t.Date)
            .IsRequired();
    }
}
