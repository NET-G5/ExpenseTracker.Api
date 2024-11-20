namespace ExpenseTracker.Api.QueryParameters;

public sealed class TransferFilter
{
    public string? Search { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int? CategoryId { get; set; }
    public int WalletId { get; set; }   
}
