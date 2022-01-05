using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionRepository(DataContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task<Transaction> GetTransactionById(string transactionId)
        {
            Transaction transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
            return transaction;
        }

        public async Task<List<Transaction>> GetAllTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();

            return transactions;
        }

        public async Task<List<Transaction>> GetUserTransactions()
        {
            List<Transaction> transactions = await _context.Transactions.Where(
                t => t.Sender == GetUserId() || t.Recipient == GetUserId()).ToListAsync();

            return transactions;
        }

        public async Task<List<Transaction>> GetWalletTransactions(string walletId)
        {
            List<Transaction> transactions = await _context.Transactions.Where(
                t => t.WalletId == walletId).ToListAsync();

            return transactions;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
