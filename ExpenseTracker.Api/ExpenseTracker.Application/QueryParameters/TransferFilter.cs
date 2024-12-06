using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Application.QueryParameters
{
    public sealed record TransferFilter(
        int CategoryId,
        int WalletId,
        decimal MinAmount,
        decimal MaxAmount);  
}
