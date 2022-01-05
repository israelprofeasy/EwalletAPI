using Ewallet.Dtos;
using Ewallet.Dtos.Currency;
using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface ICurrencyService
    {
        Task<List<Currency>> Add(Currency newCurrency);
        List<Currency> GetCurrencies();
        Task<Currency> GetCurrencyById(string id);
        Task<Currency> Update(string id, Currency currency);
        Task<bool> Delete(string id);
    }
}
