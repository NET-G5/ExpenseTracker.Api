using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.QueryParameters;

public class CategoryFilter
{
    public string Name { get; set; }
    public CategoryType Type { get; set; }
}
