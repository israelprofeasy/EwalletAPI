using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Transaction;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private readonly IWalletService _walletService;

        public TransactionController(ITransactionService transactionService, IMapper mapper, IWalletService walletService)
        {
            _transactionService = transactionService;
            _mapper = mapper;
            _walletService = walletService;
        }

        [HttpGet("get-all-transactions")]
        public async Task<IActionResult> Get()
        {
            ResponseDto<List<GetTransactionDto>> res = new ResponseDto<List<GetTransactionDto>>();
            List<GetTransactionDto> listOfTransactions = new List<GetTransactionDto>();

            List<Transaction> transactions = await _transactionService.GetAllTransactions();

            if (transactions.Count > 0)
            {
                foreach (var transaction in transactions)
                {
                    listOfTransactions.Add(_mapper.Map<GetTransactionDto>(transaction));
                }
                res.Message = "List of all transactions";
                res.Status = true;
                res.Data = listOfTransactions;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve transactions", ModelState, ""));
        }

        [HttpGet("get-user-transactions")]
        public async Task<IActionResult> GetUserTransactions()
        {
            ResponseDto<List<GetTransactionDto>> res = new ResponseDto<List<GetTransactionDto>>();
            List<GetTransactionDto> listOfTransactions = new List<GetTransactionDto>();

            List<Transaction> transactions = await _transactionService.GetUserTransactions();

            if (transactions.Count > 0)
            {
                foreach (var transaction in transactions)
                {
                    listOfTransactions.Add(_mapper.Map<GetTransactionDto>(transaction));
                }
                res.Message = "List of user's transactions";
                res.Status = true;
                res.Data = listOfTransactions;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve user's transactions", ModelState, ""));
        }

        [HttpGet("get-wallet-transactions")]
        public async Task<IActionResult> GetWalletTransactions(string walletId)
        {
            ResponseDto<List<GetTransactionDto>> res = new ResponseDto<List<GetTransactionDto>>();
            List<GetTransactionDto> listOfTransactions = new List<GetTransactionDto>();

            Wallet wallet = await _walletService.GetWalletById(walletId);

            // check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            string currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            string role = currentUser.FindFirst(ClaimTypes.Role).Value;

            if (!role.Equals("Admin") && !wallet.UserId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to edit another user's details");
                return BadRequest(Util.BuildResponse<string>(false, "Access Denied!", ModelState, ""));
            }
            else
            {
                List<Transaction> transactions = await _transactionService.GetWalletTransactions(walletId);

                if (transactions.Count > 0)
                {
                    foreach (var transaction in transactions)
                    {
                        listOfTransactions.Add(_mapper.Map<GetTransactionDto>(transaction));
                    }
                    res.Message = "List of wallet's transactions";
                    res.Status = true;
                    res.Data = listOfTransactions;
                    return Ok(res);
                }
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve wallet's transactions", ModelState, ""));
        }
    }
}
