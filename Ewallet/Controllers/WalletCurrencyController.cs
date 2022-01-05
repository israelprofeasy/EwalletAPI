using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Wallet;
using Ewallet.Dtos.WalletCurrency;
using Ewallet.Models;
using Ewallet.Services.Implementations;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    public class WalletCurrencyController : ControllerBase
    {
        private readonly IWalletCurrencyService _walletCurrencyService;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public WalletCurrencyController(IWalletCurrencyService walletCurrencyService, IWalletService walletService, IMapper mapper)
        {
            _walletCurrencyService = walletCurrencyService;
            _walletService = walletService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Noob, Elite")]
        public async Task<IActionResult> Add(AddWalletCurrencyDto model)
        {
            ResponseDto<GetWalletDto> res = new ResponseDto<GetWalletDto>();           

            ClaimsPrincipal currentUser = this.User;
            string role = currentUser.FindFirstValue(ClaimTypes.Role);
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);            

            if (ModelState.IsValid)
            {
                if(await _walletCurrencyService.CheckWalletCurrencyExist(model.WalletId, model.CurrencyId))
                {
                    ModelState.AddModelError("Failed", $"Currency already exist in the wallet");
                    return BadRequest(Util.BuildResponse<string>(false, "Unable to add to database", ModelState, ""));
                }                

                WalletCurrency newWalletCurrency = _mapper.Map<WalletCurrency>(model);

                WalletCurrency mainWallet = await _walletCurrencyService.GetMainWalletCurrency(model.WalletId);

                if (role.Equals("Noob") && mainWallet != null && model.IsMain == true) newWalletCurrency.IsMain = false;
                else if (role.Equals("Elite") && mainWallet != null && model.IsMain == true) mainWallet.IsMain = false;

                await _walletCurrencyService.Add(newWalletCurrency);

                Wallet getWallet = await _walletService.GetWalletById(model.WalletId);

                var wallet = _mapper.Map<GetWalletDto>(getWallet);

                res.Status = true;
                res.Message = "Wallet currency added successfully!";
                res.Data = wallet;

                return Ok(res);

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to add to database", ModelState, ""));
        }

        [Authorize(Roles = "Elite")]
        [HttpPatch("set-main-wallet/{walletId}/{currencyId}")]
        public async Task<IActionResult> SetMainWalletCurrency(string walletId, string currencyId, string userId)
        {
            ResponseDto<GetWalletDto> res = new ResponseDto<GetWalletDto>();

            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to update another user's wallet");
                return BadRequest(Util.BuildResponse<string>(false, "Access denied!", ModelState, ""));
            }
            else if (!await _walletService.HasWallet())
            {
                ModelState.AddModelError("Failed", $"Invalid payload");
                return BadRequest(Util.BuildResponse<string>(false, "You have no wallet!", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {

                    if(await _walletCurrencyService.SetMainWalletCurrency(walletId, currencyId))
                    {
                        Wallet getWallet = await _walletService.GetWalletById(walletId);
                        var wallet = _mapper.Map<GetWalletDto>(getWallet);

                        res.Status = true;
                        res.Message = "Main Wallet currency set successfully!";
                        res.Data = wallet;

                        return Ok(res);
                    }

                    ModelState.AddModelError("Failed", $"Invalid payload");
                    return BadRequest(Util.BuildResponse<string>(false, "Main wallet not found", ModelState, ""));
                }
            }

            ModelState.AddModelError("Denied", $"Noob Account can only have one wallet");
            return BadRequest(Util.BuildResponse<string>(false, "Access Denied", ModelState, ""));
        }
    }
}