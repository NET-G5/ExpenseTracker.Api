namespace ExpenseTracker.Api.Models;

public class Category
{
    private static int _idCounter = 0;
    private readonly List<Transfer> _transfers = new List<Transfer>();

    public int Id { get; set; }
    public string Name { get; set; }
    public CategoryType Type { get; set; }
    public IEnumerable<Transfer> Transfers => _transfers.ToList();

    public Category()
    {
        _idCounter++;

        Id = _idCounter;
    }

    public void AddTransfer(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);

        if (_transfers.Exists(t => t.Id == transfer.Id))
        {
            return;
        }

        _transfers.Add(transfer);
        transfer.Category = this;
    }
}

public enum CategoryType
{
    Income = 0,
    Expense = 1
}