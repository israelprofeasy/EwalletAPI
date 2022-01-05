using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Fund;
using Ewallet.Dtos.Transaction;
using Ewallet.Enums;
using Ewallet.Helpers;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FundController : ControllerBase
    {
        private readonly IFundService _fundService;
        private readonly IWalletService _walletService;
        private readonly ICurrencyService _currencyService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public FundController(
            IFundService fundService, IWalletService walletService, ICurrencyService currencyService, 
            ITransactionService transactionService, IMapper mapper, IEmailSender emailSender, ILoggerService loggerService)
        {
            _fundService = fundService;
            _walletService = walletService;
            _currencyService = currencyService;
            _transactionService = transactionService;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [HttpPost("fund-wallet")]
        public async Task<IActionResult> FundWallet(FundWalletDto model)
        {
            ResponseDto<GetTransactionDto> res = new ResponseDto<GetTransactionDto>();

            ClaimsPrincipal currentUser = this.User;
            string role = currentUser.FindFirstValue(ClaimTypes.Role);
            string currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                Wallet wallet = await _walletService.GetWalletById(model.WalletId);

                Transaction newTransaction = _mapper.Map<Transaction>(model);
                newTransaction.Sender = currentUserId;
                newTransaction.Recipient = wallet.UserId;
                newTransaction.Status = Status.Pending;
                newTransaction.TransactionType = TransactionType.Deposit;

                string transactionId = await _transactionService.Add(newTransaction);

                bool response = await _fundService.Fund(model);

                if (response)
                {
                    Transaction updateTransaction = _mapper.Map<Transaction>(model);
                    updateTransaction.Sender = currentUserId;
                    updateTransaction.Recipient = wallet.UserId;
                    updateTransaction.Status = Status.Success;
                    updateTransaction.TransactionType = TransactionType.Deposit;

                    Transaction result = await _transactionService.Update(transactionId, updateTransaction);

                    if (result != null)
                    {
                        GetTransactionDto updatedTransaction = _mapper.Map<GetTransactionDto>(result);

                        res.Status = true;
                        res.Message = "Transaction successful";
                        res.Data = updatedTransaction;

                        // Sends confirmation mail to user on every successful transaction
                        var email = currentUser.FindFirstValue(ClaimTypes.Name);
                        var currency = await _currencyService.GetCurrencyById(updatedTransaction.CurrencyId);
                        //var currencyCode = currency.ShortCode;
                        var currencyCode = result.Currency.ShortCode;

                        string transactionNotification = $"<p>Hi {email},</p>" +
                        $"<p>You just performed a transaction. Below are the details of the transaction</p>" +
                        $"<p><b>Amount:</b> {currencyCode} {updatedTransaction.Amount}<br/>" +
                        $"<b>Wallet:</b> {wallet.Name}<br/> " +
                        $"<b>Description:</b> {updatedTransaction.Description}<br/>" +
                        $"<b>Transaction Type:</b> {updatedTransaction.TransactionType}<br/> " +
                        $"<b>Transaction Status:</b> {updatedTransaction.Status}<br/> " +
                        $"<b>Transaction Date:</b> {updatedTransaction.TransactionDate} </p>" +
                        $"<p>Best regards,<br/>Merlin</p>" +
                        $"<p><b>Note:</b> You are receiving this email because it's important and it's not something that you can unsubscribe from.</p>";

                        var message = new Message(new List<EmailConfiguration> { new EmailConfiguration { 
                            DisplayName = string.Empty, 
                            From = "no_reply@gmail.com", 
                            Address = email
                        } }, "Deposit Transaction", transactionNotification);

                        await _emailSender.SendEmailAsync(message);
                    }
                    else
                    {
                        ModelState.AddModelError("Failed", $"Invalid payload");
                        return BadRequest(Util.BuildResponse<string>(false, "Unable to update transaction", ModelState, ""));
                    }

                }
                else
                {
                    Transaction updateTransaction = new Transaction
                    {
                        Status = Status.Declined
                    };

                    Transaction result = await _transactionService.Update(transactionId, updateTransaction);

                    if (result != null)
                    {
                        ModelState.AddModelError("Failed", $"Transaction declined");
                        return BadRequest(Util.BuildResponse<string>(false, "Unable to fund wallet", ModelState, ""));
                    }
                }

                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to fund wallet", ModelState, ""));
        }


        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawDto model)
        {
            ResponseDto<GetTransactionDto> res = new ResponseDto<GetTransactionDto>();

            ClaimsPrincipal currentUser = this.User;
            string role = currentUser.FindFirstValue(ClaimTypes.Role);
            string currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                Wallet wallet = await _walletService.GetWalletById(model.WalletId);

                Transaction newTransaction = _mapper.Map<Transaction>(model);
                newTransaction.Sender = currentUserId;
                newTransaction.Recipient = wallet.UserId;
                newTransaction.Status = Status.Pending;
                newTransaction.TransactionType = TransactionType.Withdrawal;

                string transactionId = await _transactionService.Add(newTransaction);

                bool response = await _fundService.Withdraw(model);

                if (response)
                {
                    Transaction updateTransaction = _mapper.Map<Transaction>(model);
                    updateTransaction.Sender = currentUserId;
                    updateTransaction.Recipient = wallet.UserId;
                    updateTransaction.Status = Status.Success;
                    updateTransaction.TransactionType = TransactionType.Withdrawal;

                    Transaction result = await _transactionService.Update(transactionId, updateTransaction);

                    if (result != null)
                    {
                        GetTransactionDto updatedTransaction = _mapper.Map<GetTransactionDto>(result);

                        res.Status = true;
                        res.Message = "Transaction successful";
                        res.Data = updatedTransaction;

                        // Sends confirmation mail to user on every successful transaction
                        var email = currentUser.FindFirstValue(ClaimTypes.Name);
                        var currency = await _currencyService.GetCurrencyById(updatedTransaction.CurrencyId);
                        var currencyCode = currency.ShortCode;

                        string transactionNotification = $"<p>Hi {email},</p>" +
                        $"<p>You just performed a transaction. Below are the details of the transaction</p>" +
                        $"<p><b>Amount:</b> {currencyCode} {updatedTransaction.Amount}<br/>" +
                        $"<b>Wallet:</b> {wallet.Name}<br/> " +
                        $"<b>Description:</b> {updatedTransaction.Description}<br/>" +
                        $"<b>Transaction Type:</b> {updatedTransaction.TransactionType}<br/> " +
                        $"<b>Transaction Status:</b> {updatedTransaction.Status}<br/> " +
                        $"<b>Transaction Date:</b> {updatedTransaction.TransactionDate} </p>" +
                        $"<p>Best regards,<br/>Merlin</p>" +
                        $"<p><b>Note:</b> You are receiving this email because it's important and it's not something that you can unsubscribe from.</p>";

                        var message = new Message(new List<EmailConfiguration> { new EmailConfiguration {
                            DisplayName = string.Empty,
                            From = "no_reply@gmail.com",
                            Address = email
                        } }, "Deposit Transaction", transactionNotification);

                        await _emailSender.SendEmailAsync(message);
                    }
                    else
                    {
                        ModelState.AddModelError("Failed", $"Invalid payload");
                        return BadRequest(Util.BuildResponse<string>(false, "Unable to update transaction", ModelState, ""));
                    }

                }
                else
                {
                    Transaction updateTransaction = new Transaction
                    {
                        Status = Status.Declined
                    };

                    Transaction result = await _transactionService.Update(transactionId, updateTransaction);

                    if (result != null)
                    {
                        ModelState.AddModelError("Failed", $"Transaction declined");
                        return BadRequest(Util.BuildResponse<string>(false, "Unable to withdraw from wallet", ModelState, ""));
                    }
                }

                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to withdraw from wallet", ModelState, ""));
        }
    }
}
