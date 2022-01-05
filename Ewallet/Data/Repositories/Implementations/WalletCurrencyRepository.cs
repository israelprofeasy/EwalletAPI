using Ewallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public class WalletCurrencyRepository : IWalletCurrencyRepository
    {
        private readonly DataContext _context;

        public WalletCurrencyRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Add<T>(T entity)
        {
            _context.Add(entity);
            return await SaveChanges();
        }

        public async Task<bool> Delete<T>(T entity)
        {
            _context.Remove(entity);
            return await SaveChanges();
        }

        public async Task <bool> Edit<T>(T entity)
        {
            _context.Update(entity);
            return await SaveChanges();
        }

        public async Task<List<WalletCurrency>> GetUserWalletCurrencies(string walletId)
        {
            List<WalletCurrency> walletCurrencies = await _context.WalletCurrency.Where(
                x => x.WalletId == walletId).ToListAsync();

            return walletCurrencies;
        }

        /*
        public async Task<List<Wallet>> GetWalletCurrencies(string walletId)
        {
            List<Wallet> wallets = await _context.Wallets.Include(w => w.WalletCurrencies).ThenInclude(wc => wc.Currency).Where(
                w => w.Id == walletId).ToListAsync();

            return wallets;
        }
        */

        public async Task<WalletCurrency> CheckWalletCurrencyExist(string walletId, string currencyId)
        {

            WalletCurrency walletCurrency = await _context.WalletCurrency.FirstOrDefaultAsync(wc => wc.WalletId == walletId && wc.CurrencyId == currencyId);

            return walletCurrency;
        }

        public async Task<WalletCurrency> GetMainWalletCurrency(string walletId)
        {
            return await _context.WalletCurrency.Where(wc => wc.WalletId == walletId).FirstOrDefaultAsync(wc => wc.IsMain == true);
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
