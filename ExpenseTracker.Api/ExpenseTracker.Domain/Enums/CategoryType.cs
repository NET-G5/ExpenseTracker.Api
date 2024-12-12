using System.Text.Json.Serialization;

namespace ExpenseTracker.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryType
{
    Income,
    Expense
}
