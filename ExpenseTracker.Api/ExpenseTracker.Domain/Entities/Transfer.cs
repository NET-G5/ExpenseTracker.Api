using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Transfer : AuditableEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; }

    public int WalletId { get; set; }
    public virtual Wallet Wallet { get; set; }
}
