using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyConverterService _currencyConverterService;
        private readonly IWalletCurrencyService _walletCurrencyService;

        public WalletService(IWalletRepository walletRepository,
            ICurrencyConverterService currencyConverterService, IWalletCurrencyService walletCurrencyService)
        {
            _walletRepository = walletRepository;
            _currencyConverterService = currencyConverterService;
            _walletCurrencyService = walletCurrencyService;
        }
        public async Task<List<Wallet>> Add(Wallet newWallet)
        {
            try
            {
                Wallet mainWallet = await _walletRepository.GetMainWallet();

                if (mainWallet != null)
                {
                    mainWallet.IsMain = false;
                    newWallet.IsMain = true;
                }

                await _walletRepository.Add(newWallet);
                var result = await _walletRepository.GetUserWallets();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(string walletId)
        {
            bool status = false;

            try
            {
                Wallet wallet = await _walletRepository.GetWalletById(walletId);
                if (await _walletRepository.Delete(wallet))
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                // log err
                throw new Exception(ex.Message);
            }

            return status;
        }

        public async Task<List<Wallet>> GetUserWallets()
        {
            List<Wallet> wallets = new List<Wallet>();

            try
            {
                wallets = await _walletRepository.GetUserWallets();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return wallets;
        }

        public async Task<Wallet> GetWalletById(string id)
        {
            Wallet wallet = null;
            try
            {
                wallet = await _walletRepository.GetWalletById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return wallet;
        }

        public async Task<List<Wallet>> GetWallets()
        {
            List<Wallet> wallets = new List<Wallet>();

            try
            {
                wallets = await _walletRepository.GetAllWallets();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return wallets;
        }

        public async Task<Wallet> Update(string walletId, string userId, Wallet wallet)
        {
            Wallet updatedWallet = null;

            try
            {
                Wallet getWallet = await _walletRepository.GetWalletById(walletId);
                if (getWallet != null)
                {
                    getWallet.Name = wallet.Name;
                    getWallet.IsMain = wallet.IsMain;
                }

                if (wallet.IsMain) await SetMainWallet(walletId);

                if (await _walletRepository.Edit(getWallet))
                {
                    updatedWallet = new Wallet
                    {
                        Id = wallet.Id,
                        Name = wallet.Name,
                        UserId = userId,
                        IsMain = wallet.IsMain
                    };
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return updatedWallet;
        }

        public async Task<Wallet> GetMainWallet()
        {
            Wallet mainWallet = await _walletRepository.GetMainWallet();
            return mainWallet;
        }

        public async Task<bool> SetMainWallet(string walletId)
        {
            bool status = false;
            Wallet wallet = await _walletRepository.GetWalletById(walletId);
            Wallet mainWallet = await _walletRepository.GetMainWallet();

            if (wallet != null)
            {
                if (mainWallet != null)
                {
                    mainWallet.IsMain = false;
                    wallet.IsMain = true;
                    await _walletRepository.Edit(wallet);
                    status = true;
                }
                else
                {
                    wallet.IsMain = true;
                    await _walletRepository.Edit(wallet);
                    status = true;
                }
            }

            return status;
        }

        public async Task<bool> HasWallet()
        {
            bool status = false;

            Wallet wallet = await _walletRepository.HasWallet();

            if (wallet != null) status = true;
            return status;
        }

        public async Task MergeWallets(string userId)
        {
            List<Wallet> wallets = await _walletRepository.GetUserWalletsByAdmin(userId);
            Wallet mainWallet = wallets.FirstOrDefault(w => w.IsMain);

            foreach (var wallet in wallets)
            {
                if (!wallet.IsMain)
                {
                    var fromWalletCurrency = await _walletCurrencyService.GetMainWalletCurrency(wallet.Id);
                    var toWalletCurrency = await _walletCurrencyService.GetMainWalletCurrency(mainWallet.Id);
                    var exchangeRate = _currencyConverterService.GetCurrencyExchange(fromWalletCurrency.Currency.ShortCode, toWalletCurrency.Currency.ShortCode);
                    var conversion = wallet.Balance * exchangeRate;
                    mainWallet.Balance += conversion;
                    await Delete(wallet.Id);
                }
                else
                {
                    throw new Exception("Main wallet not found. Please set a main wallet");
                }
            }

        }
    }
}
