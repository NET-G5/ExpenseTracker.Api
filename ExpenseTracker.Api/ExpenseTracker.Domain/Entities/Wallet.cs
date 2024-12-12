using ExpenseTracker.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Domain.Entities;

public class Wallet : AuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Balance { get; set; }

    public Guid OwnerId { get; set; }
    public required virtual IdentityUser<Guid> Owner { get; set; }

    public virtual ICollection<Transfer> Transfers { get; set; }

    public Wallet()
    {
        Transfers = [];
    }
}
