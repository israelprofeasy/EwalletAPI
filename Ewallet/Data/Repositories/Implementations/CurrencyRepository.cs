using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly DataContext _context;

        public CurrencyRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Add<T>(T entity)
        {
            Currency currency = entity as Currency;
            try
            {
                await _context.Currencies.AddAsync(currency);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete<T>(T entity)
        {
            string id = entity as string;

            bool status = false;

            try
            {
                var currency = await GetCurrencyById(id);

                if (currency != null)
                {
                    _context.Currencies.Remove(currency);
                    await _context.SaveChangesAsync();
                    status = true;
                }   
            }
            catch (DbException ex)
            {

                throw new Exception(ex.Message);
            }

            return status;
        }

        public async Task<bool> Edit<T>(T entity)
        {
            string id = entity as string;

            bool status = false;

            try
            {
                var currency = await GetCurrencyById(id);

                if (currency != null)
                {
                    _context.Currencies.Update(currency);
                    await _context.SaveChangesAsync();
                    status = true;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return status;
        }

        public List<Currency> GetAllCurrencies()
        {
            List<Currency> currencies =  _context.Currencies.ToList();
            return currencies;
        }

        public async Task<Currency> GetCurrencyById(string id)
        {
            Currency currency = null;
            try
            {
                currency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return currency;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
