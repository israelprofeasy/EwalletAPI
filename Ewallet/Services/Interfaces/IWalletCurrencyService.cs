using Ewallet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public interface IWalletCurrencyService
    {
        Task<List<WalletCurrency>> Add(WalletCurrency newWalletCurrency);
        Task<List<WalletCurrency>> GetUserWalletCurrencies(string walletId);
        Task<WalletCurrency> GetWalleCurrencyById(string walletCurrencyId);
        Task<WalletCurrency> GetMainWalletCurrency(string walletId);
        Task<WalletCurrency> Update(string walletCurrencyId, string walletId, string userId, WalletCurrency walletCurrency);
        Task<bool> CheckWalletCurrencyExist(string walletId, string currencyId);
        Task<bool> SetMainWalletCurrency(string walletId, string currencyId);
        Task<bool> Delete(string walletCurrencyId);
    }
}