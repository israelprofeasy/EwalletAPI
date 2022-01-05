using Ewallet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public interface IWalletCurrencyRepository : ICRUDRepository
    {
        Task<List<WalletCurrency>> GetUserWalletCurrencies(string walletId);
        Task<WalletCurrency> CheckWalletCurrencyExist(string walletId, string currencyId);
        Task<WalletCurrency> GetMainWalletCurrency(string walletId);
    }
}