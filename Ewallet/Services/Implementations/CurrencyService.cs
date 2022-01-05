using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Dtos;
using Ewallet.Dtos.Currency;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }
        public async Task<List<Currency>> Add(Currency newCurrency)
        {
            try
            {
                await _currencyRepository.Add(newCurrency);
                var result = _currencyRepository.GetAllCurrencies();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(string id)
        {
            bool status = false;

            try
            {
                if (await _currencyRepository.Delete<string>(id))
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

        public List<Currency> GetCurrencies()
        {
            try
            {
                var result = _currencyRepository.GetAllCurrencies();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<Currency> GetCurrencyById(string id)
        {
            Currency currency = null;
            try
            {
                currency = await _currencyRepository.GetCurrencyById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return currency;
        }

        public async Task<Currency> Update(string id, Currency currency)
        {
            Currency updatedCurrency = null;

            try
            {
                Currency getCurrency = await _currencyRepository.GetCurrencyById(id);
                if (getCurrency != null)
                {
                    getCurrency.Name = currency.Name;
                    getCurrency.ShortCode = currency.ShortCode;

                }
                if (await _currencyRepository.Edit<string>(id))
                {
                    updatedCurrency = new Currency
                    {
                        Id = currency.Id,
                        Name = currency.Name,
                        ShortCode = currency.ShortCode
                    };
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return updatedCurrency;
        }
    }
}
