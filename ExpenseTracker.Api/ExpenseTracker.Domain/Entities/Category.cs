using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Domain.Entities;

public class Category : AuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public CategoryType Type { get; set; }

    public Guid OwnerId { get; set; }
    public required virtual IdentityUser<Guid> Owner { get; set; }

    public virtual ICollection<Transfer> Transfers { get; set; }

    public Category()
    {
        Transfers = [];
    }
}
