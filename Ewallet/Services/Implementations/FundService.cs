using Ewallet.Data.Repositories.Implementations;
using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Dtos.Fund;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class FundService : IFundService
    {
        private readonly IWalletCurrencyRepository _walletCurrencyRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyConverterService _currencyConverterService;

        public FundService(IWalletCurrencyRepository walletCurrencyRepository, IWalletRepository walletRepository, ICurrencyConverterService currencyConverterService)
        {
            _walletCurrencyRepository = walletCurrencyRepository;
            _walletRepository = walletRepository;
            _currencyConverterService = currencyConverterService;
        }
        public async Task<bool> Fund(FundWalletDto request)
        {
            bool status = false;
            WalletCurrency walletCurrency = await _walletCurrencyRepository.CheckWalletCurrencyExist(request.WalletId, request.CurrencyId);

            try
            {
                if (walletCurrency != null)
                {
                    Wallet wallet = await _walletRepository.GetWalletById(request.WalletId);

                    if (wallet != null)
                    {
                        WalletCurrency mainCurrency = await _walletCurrencyRepository.GetMainWalletCurrency(request.WalletId);
                        var toSymbol = mainCurrency.Currency.ShortCode;
                        var fromSymbol = walletCurrency.Currency.ShortCode;

                        var exchangeRate = _currencyConverterService.GetCurrencyExchange(fromSymbol, toSymbol);
                        var conversion = request.Amount * exchangeRate;
                        //var conversion = _currencyConverterService.FixerAPIConversion(fromSymbol, toSymbol, request.Amount);

                        //wallet.Balance += request.Amount;
                        wallet.Balance += conversion;
                        walletCurrency.Balance += request.Amount;
                        await _walletCurrencyRepository.Edit(walletCurrency);
                        await _walletRepository.Edit(wallet);

                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }            

            return status;
        }

        public async Task<bool> Withdraw(WithdrawDto request)
        {
            bool status = false;
            WalletCurrency walletCurrency = await _walletCurrencyRepository.CheckWalletCurrencyExist(request.WalletId, request.CurrencyId);

            try
            {
                if (walletCurrency != null)
                {
                    Wallet wallet = await _walletRepository.GetWalletById(request.WalletId);

                    if (wallet != null)
                    {
                        WalletCurrency mainCurrency = await _walletCurrencyRepository.GetMainWalletCurrency(request.WalletId);
                        var toSymbol = mainCurrency.Currency.ShortCode;
                        var fromSymbol = walletCurrency.Currency.ShortCode;

                        var exchangeRate = _currencyConverterService.GetCurrencyExchange(fromSymbol, toSymbol);

                        var conversion = request.Amount * exchangeRate;

                        if(walletCurrency.Balance < request.Amount)
                        {
                            wallet.Balance += walletCurrency.Balance * exchangeRate;
                            walletCurrency.Balance = 0;

                            wallet.Balance -= conversion;
                            await _walletCurrencyRepository.Edit(walletCurrency);
                            await _walletRepository.Edit(wallet);
                        }
                        else
                        {
                            //wallet.Balance += request.Amount;
                            wallet.Balance -= conversion;
                            walletCurrency.Balance -= request.Amount;
                            await _walletCurrencyRepository.Edit(walletCurrency);
                            await _walletRepository.Edit(wallet);
                        }                        

                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return status;

        }
    }
}
