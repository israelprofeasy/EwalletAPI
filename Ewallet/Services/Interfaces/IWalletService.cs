using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface IWalletService
    {
        Task<List<Wallet>> Add(Wallet newWallet);
        Task<List<Wallet>> GetWallets();
        Task<List<Wallet>> GetUserWallets();
        Task<Wallet> GetWalletById(string walletId);
        Task<Wallet> GetMainWallet();
        Task<Wallet> Update(string walletId, string userId, Wallet wallet);
        Task<bool> SetMainWallet(string walletId);
        Task<bool> HasWallet();
        Task MergeWallets(string userId);
        Task<bool> Delete(string walletId);
    }
}
