using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Interfaces
{
    public interface ICurrencyRepository : ICRUDRepository
    {
        List<Currency> GetAllCurrencies();
        Task<Currency> GetCurrencyById(string id);
    }
}
