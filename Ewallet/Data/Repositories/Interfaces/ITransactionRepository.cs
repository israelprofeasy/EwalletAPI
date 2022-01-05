using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Interfaces
{
    public interface ITransactionRepository: ICRUDRepository
    {
        Task<Transaction> GetTransactionById(string transactionId);
        Task<List<Transaction>> GetWalletTransactions(string walletId);
        Task<List<Transaction>> GetUserTransactions();
        Task<List<Transaction>> GetAllTransactions();
    }
}
