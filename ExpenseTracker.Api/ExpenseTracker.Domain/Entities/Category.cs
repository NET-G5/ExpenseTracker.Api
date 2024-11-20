using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Domain.Entities;

public class Category : AuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public CategoryType Type { get; set; }

    public virtual ICollection<Transfer> Transfers { get; set; }

    public Category()
    {
        Transfers = [];
    }
}
