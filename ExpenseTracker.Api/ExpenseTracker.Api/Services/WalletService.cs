using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;
using ExpenseTracker.Api.Services.Interfaces;
using System.Data;

namespace ExpenseTracker.Api.Services
{

    internal sealed  class WalletService : IWalletService
    {
        private readonly static List<Wallet> _wallets = []; 
        public Task<Wallet> CreateAsync(Wallet wallet)
        {
            ArgumentNullException.ThrowIfNull(wallet);
           
            _wallets.Add(wallet);   
            return Task.FromResult(wallet);
        }

        public Task DeleteAsync(int id)
        {
            var walletToDelete = _wallets.Find(x => x.Id == id);

            if (walletToDelete is not null)
            {
                _wallets.Remove(walletToDelete);
            }

            return Task.CompletedTask;
        }

        public Task<List<Wallet>> GetAsync(WalletFilter? filter = null)
        {
            if (filter is null)
            {
                return Task.FromResult(_wallets);
            }

            var wallets = _wallets
               .Where(x => x.Name.Contains(filter.Name))
               .ToList();   

            return Task.FromResult(wallets);
        }

        public Task<Wallet> GetByIdAsync(int id)
         => Task.FromResult(_wallets.Find(x => x.Id == id));

        public Task UpdateAsync(Wallet wallet)
        {
            var walletToUpdate = _wallets.Find(x => x.Id == wallet.Id);

            if (walletToUpdate is not null)
            {
                walletToUpdate.Name = wallet.Name;
              
            }

            return Task.CompletedTask;
        }
    }
}
