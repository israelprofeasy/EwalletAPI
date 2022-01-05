using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionService(ITransactionRepository transactionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _transactionRepository = transactionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        public async Task<string> Add(Transaction newTransaction)
        {
            string transactionId = null;

            if (await _transactionRepository.Add(newTransaction))
            {
                transactionId = newTransaction.Id;
            }

            return transactionId;
        }
        
        public async Task<List<Transaction>> GetAllTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                transactions = await _transactionRepository.GetAllTransactions();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return transactions;
        }

        public async Task<bool> Delete(string transactionId)
        {
            bool status = false;

            try
            {
                Transaction transaction = await _transactionRepository.GetTransactionById(transactionId);

                if (await _transactionRepository.Delete(transaction))
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

        public async Task<Transaction> GetTransactionById(string transactionId)
        {
            Transaction transaction = null;
            try
            {
                string currentUserId = GetUserId();
                string role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

                Transaction getTransaction = await _transactionRepository.GetTransactionById(transactionId);

                if (!role.Equals("Admin") && !getTransaction.Sender.Equals(currentUserId) && !getTransaction.Recipient.Equals(currentUserId))
                {
                    throw new Exception("Unauthorized Access");
                }

                transaction = getTransaction;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return transaction;
        }

        public async Task<List<Transaction>> GetUserTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                transactions = await _transactionRepository.GetUserTransactions();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return transactions;
        }

        public async Task<List<Transaction>> GetWalletTransactions(string walletId)
        {
            List<Transaction> transactions = new List<Transaction>();

            try
            {
                transactions = await _transactionRepository.GetWalletTransactions(walletId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return transactions;
        }

        public async Task<Transaction> Update(string transactionId, Transaction updatedTransaction)
        {
            Transaction transaction = null;
            Transaction getTransaction = await GetTransactionById(transactionId);

            if (getTransaction != null)
            {
                // getTransaction.Id = updatedTransaction.Id;
                getTransaction.Amount = updatedTransaction.Amount;
                getTransaction.Description = updatedTransaction.Description;
                getTransaction.WalletId = updatedTransaction.WalletId;
                getTransaction.CurrencyId = updatedTransaction.CurrencyId;
                getTransaction.Sender = updatedTransaction.Sender;
                getTransaction.Recipient = updatedTransaction.Recipient;
                getTransaction.Status = updatedTransaction.Status;
                getTransaction.TransactionType = updatedTransaction.TransactionType;

                if(await _transactionRepository.Edit(getTransaction))
                {
                    transaction = new Transaction
                    {
                        Id = updatedTransaction.Id,
                        Amount = updatedTransaction.Amount,
                        Description = updatedTransaction.Description,
                        WalletId = updatedTransaction.WalletId,
                        CurrencyId = updatedTransaction.CurrencyId,
                        Sender = updatedTransaction.Sender,
                        Recipient = updatedTransaction.Recipient,
                        Status = updatedTransaction.Status,
                        TransactionType = updatedTransaction.TransactionType
                    };
                    
                    // await GetTransactionById(updatedTransaction.Id);
                }
            }

            return transaction;
        }

    }
}
