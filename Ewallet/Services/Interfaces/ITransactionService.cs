using Ewallet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<string> Add(Transaction newTransaction);
        Task<List<Transaction>> GetAllTransactions();
        Task<List<Transaction>> GetWalletTransactions(string walletId);
        Task<Transaction> GetTransactionById(string transactionId);
        Task<List<Transaction>> GetUserTransactions();
        Task<Transaction> Update(string transactionId, Transaction updatedTransaction);
        Task<bool> Delete(string transactionId);
    }
}