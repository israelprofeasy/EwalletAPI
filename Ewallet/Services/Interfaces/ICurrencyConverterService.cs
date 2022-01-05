using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface ICurrencyConverterService
    {
        decimal GetCurrencyExchange(string fromCurrency, string toCurrency);
        decimal FixerAPIConversion(string fromCurrency, string toCurrency, decimal amount);
    }
}
