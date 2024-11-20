namespace ExpenseTracker.Api.Models;

public class Transfer
{
    private static int _idCounter = 0;

    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }

    public int CategoryId { get; set; }

    private Category? _category;
    public int WalletId { get; set; }
    private Wallet? _wallet;
    public Category? Category
    { 
        get => _category;
        set
        {
            if (_category == value) return;

            _category = value;
            _category.AddTransfer(this);
        }
    }

    public Wallet? Wallet 
    {
        get => _wallet;
        set
        {
            if(_wallet == value) return;
            _wallet = value;
            _wallet.AddTransfer(this);
        }
    }

    public Transfer()
    {
        _idCounter++;
        Id = _idCounter;
    }
}
