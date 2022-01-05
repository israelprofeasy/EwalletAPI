using Ewallet.Data.Repositories.Implementations;
using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class WalletCurrencyService : IWalletCurrencyService
    {
        private readonly IWalletCurrencyRepository _walletCurrencyRepository;

        public WalletCurrencyService(IWalletCurrencyRepository walletCurrencyRepository)
        {
            _walletCurrencyRepository = walletCurrencyRepository;
        }
        public async Task<List<WalletCurrency>> Add(WalletCurrency newWalletCurrency)
        {
            List<WalletCurrency> walletCurrencies = null;

            if(await _walletCurrencyRepository.Add(newWalletCurrency))
            {
                walletCurrencies = await _walletCurrencyRepository.GetUserWalletCurrencies(newWalletCurrency.WalletId);
            }

            return walletCurrencies;
        }

        public Task<bool> Delete(string walletCurrencyId)
        {
            throw new NotImplementedException();
        }

        public async Task<WalletCurrency> GetMainWalletCurrency(string walletId)
        {
            WalletCurrency walletCurrency = await _walletCurrencyRepository.GetMainWalletCurrency(walletId);

            if (walletCurrency != null) return walletCurrency;

            return null;
        }

        public Task<List<WalletCurrency>> GetUserWalletCurrencies(string walletId)
        {
            throw new NotImplementedException();
        }

        public Task<WalletCurrency> GetWalleCurrencyById(string walletCurrencyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SetMainWalletCurrency(string walletId, string currencyId)
        {
            bool status = false;
            WalletCurrency walletCurrency = await _walletCurrencyRepository.CheckWalletCurrencyExist(walletId, currencyId);            

            if (walletCurrency != null)
            {
                WalletCurrency mainWallet = await _walletCurrencyRepository.GetMainWalletCurrency(walletId);

                if (mainWallet != null)
                {
                    mainWallet.IsMain = false;
                    walletCurrency.IsMain = true;
                    await _walletCurrencyRepository.Edit(walletCurrency);
                    status = true;
                }
                else
                {
                    walletCurrency.IsMain = true;
                    await _walletCurrencyRepository.Edit(walletCurrency);
                    status = true;
                }
            }

            return status;
        }

        public Task<WalletCurrency> Update(string walletCurrencyId, string walletId, string userId, WalletCurrency walletCurrency)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckWalletCurrencyExist(string walletId, string currencyId)
        {
            bool status = false;

            WalletCurrency walletCurrency = await _walletCurrencyRepository.CheckWalletCurrencyExist(walletId, currencyId);

            if (walletCurrency != null) status = true;

            return status;
        }
    }
}
