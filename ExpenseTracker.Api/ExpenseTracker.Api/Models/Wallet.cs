namespace ExpenseTracker.Api.Models
{
    public class Wallet
    {
        private static int _idCounter = 0;
        private readonly List<Transfer> _transfers = new List<Transfer>();
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }

        public IEnumerable<Transfer> Transfers => _transfers.ToList();

        public Wallet()
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
            transfer.Wallet = this;
        }

    }
}
