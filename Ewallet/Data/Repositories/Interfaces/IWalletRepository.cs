using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Interfaces
{
    public interface IWalletRepository : ICRUDRepository
    {
        Task<List<Wallet>> GetAllWallets();
        Task<List<Wallet>> GetUserWallets();
        Task<List<Wallet>> GetUserWalletsByAdmin(string userId);
        Task<Wallet> GetWalletById(string id);
        Task<Wallet> GetMainWallet();
        Task<Wallet> HasWallet();
    }
}
