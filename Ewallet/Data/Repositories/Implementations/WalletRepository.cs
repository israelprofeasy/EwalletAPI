using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        public async Task<bool> Add<T>(T entity)
        {

            await _context.AddAsync(entity);
            return await SaveChanges();
        }

        public async Task<bool> Delete<T>(T entity)
        {
            _context.Remove(entity);
            return await SaveChanges();
        }

        public async Task<bool> Edit<T>(T entity)
        {
            _context.Update(entity);
            return await SaveChanges();
        }

        public async Task<List<Wallet>> GetAllWallets()
        {
            List<Wallet> wallets = await _context.Wallets
                .Include(w => w.WalletCurrencies)
                .ThenInclude(wc => wc.Currency)
                .ToListAsync();

            return wallets;
        }

        public async Task<List<Wallet>> GetUserWallets()
        {
            List<Wallet> wallets = await _context.Wallets
                .Include(w => w.WalletCurrencies).ThenInclude(wc => wc.Currency)
                .Where(x => x.User.Id == GetUserId().ToString()).ToListAsync();

            return wallets;
        }

        public async Task<List<Wallet>> GetUserWalletsByAdmin(string userId)
        {
            return await _context.Wallets.Where(w => w.UserId == userId).ToListAsync();
        }

        public async Task<Wallet> GetWalletById(string id)
        {
            Wallet wallet = null;

            // check if user logged is the one making the changes - only works for system using Auth tokens

            string currentUserId = GetUserId();
            string role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

            Wallet getWallet = await _context.Wallets.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);

            if (!role.Equals("Admin") && !getWallet.UserId.Equals(currentUserId))
            {
                throw new Exception("Unauthorized Access");
            }
            else
            {
                try
                {
                    wallet = await _context.Wallets
                        .Include(w => w.WalletCurrencies)
                        .ThenInclude(wc => wc.Currency)
                        .FirstOrDefaultAsync(w => w.Id == id);
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }

            return wallet;
        }

        public async Task<Wallet> GetMainWallet()
        {
            return await _context.Wallets.Where(w => w.UserId == GetUserId()).FirstOrDefaultAsync(w => w.IsMain == true);
        }

        public async Task<Wallet> HasWallet()
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == GetUserId());
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
