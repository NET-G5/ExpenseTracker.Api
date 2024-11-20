using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

public class Wallet : AuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }

    public virtual ICollection<Transfer> Transfers { get; set; }

    public Wallet()
    {
        Transfers = [];
    }
}
