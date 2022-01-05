using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Wallet;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;

        public WalletController(IWalletService walletService, IMapper mapper)
        {
            _walletService = walletService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Noob, Elite")]
        public async Task<IActionResult> Add(AddWalletDto model)
        {
            ResponseDto<List<GetWalletDto>> res = new ResponseDto<List<GetWalletDto>>();

            List<GetWalletDto> listOfWallets = new List<GetWalletDto>();

            ClaimsPrincipal currentUser = this.User;
            string role = currentUser.FindFirstValue(ClaimTypes.Role);
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (role.Equals("Noob") && await _walletService.HasWallet())
            {
                ModelState.AddModelError("Denied", $"Noob Account can only have one wallet");
                return BadRequest(Util.BuildResponse<string>(false, "Access Denied", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {

                    Wallet newWallet = _mapper.Map<Wallet>(model);
                    newWallet.UserId = userId;

                    if (role.Equals("Noob"))
                    {
                        newWallet.IsMain = true;
                    }

                    List<Wallet> wallets = await _walletService.Add(newWallet);

                    if (wallets.Count > 0)
                    {
                        foreach (var wallet in wallets)
                        {
                            listOfWallets.Add(_mapper.Map<GetWalletDto>(wallet));
                        }

                        res.Status = true;
                        res.Message = "Wallet added successfully!";
                        res.Data = listOfWallets;
                        return Ok(res);
                    }
                    else
                    {
                        ModelState.AddModelError("Failed", $"Invalid payload");
                        return BadRequest(Util.BuildResponse<string>(false, "Unable to add wallet", ModelState, ""));
                    }

                }
            }            

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to add to database", ModelState, ""));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-wallets")]
        public async Task<IActionResult> GetAllWallets()
        {
            ResponseDto<List<GetWalletDto>> res = new ResponseDto<List<GetWalletDto>>();
            List<GetWalletDto> listOfWallets = new List<GetWalletDto>();

            List<Wallet> wallets = await _walletService.GetWallets();

            if (wallets.Count > 0)
            {
                foreach (var wallet in wallets)
                {
                    listOfWallets.Add(_mapper.Map<GetWalletDto>(wallet));
                }
                res.Message = "List of wallets";
                res.Status = true;
                res.Data = listOfWallets;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve wallets", ModelState, ""));
        }

        [Authorize(Roles = "Noob, Elite")]
        [HttpGet("get-user-wallets")]
        public async Task<IActionResult> GetUserWallets()
        {
            ResponseDto<List<GetWalletDto>> res = new ResponseDto<List<GetWalletDto>>();
            List<GetWalletDto> listOfWallets = new List<GetWalletDto>();

            List<Wallet> wallets = await _walletService.GetUserWallets();

            if (wallets.Count > 0)
            {
                foreach (var wallet in wallets)
                {
                    listOfWallets.Add(_mapper.Map<GetWalletDto>(wallet));
                }
                res.Message = "List of user wallets";
                res.Status = true;
                res.Data = listOfWallets;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve user wallets", ModelState, ""));
        }

        [HttpGet("get-wallet-by-id")]
        public async Task<IActionResult> GetWalletById(string walletId)
        {
            ResponseDto<GetWalletDto> res = new ResponseDto<GetWalletDto>();
            GetWalletDto model = new GetWalletDto();

            Wallet wallet = await _walletService.GetWalletById(walletId);

            if (wallet != null)
            {
                model = _mapper.Map<GetWalletDto>(wallet);

                res.Status = true;
                res.Message = "Wallet details";
                res.Data = model;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve wallet", ModelState, ""));
        }

        [Authorize(Roles = "Noob, Elite")]
        [HttpPut("update-wallet/{walletId}")]
        public async Task<IActionResult> Update(string walletId, string userId, UpdateWalletDto model)
        {
            ResponseDto<GetWalletDto> res = new ResponseDto<GetWalletDto>();

            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to upload photo for another user");
                return BadRequest(Util.BuildResponse<string>(false, "Access denied!", ModelState, ""));
            }

            Wallet wallet = await _walletService.GetWalletById(walletId);

            if (wallet == null)
            {
                ModelState.AddModelError("Failed", $"Unable to update wallet");
                return BadRequest(Util.BuildResponse<string>(false, "Failed to update database", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Wallet updateWallet = new Wallet
                    {
                        Name = model.Name,
                        IsMain = model.IsMain
                    };

                    Wallet response = await _walletService.Update(walletId, userId, updateWallet);

                    if (response != null)
                    {
                        GetWalletDto updatedWallet = _mapper.Map<GetWalletDto>(response);

                        res.Status = true;
                        res.Message = "Wallet updated";
                        res.Data = updatedWallet;
                    }

                    return Ok(res);
                }
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to update wallet", ModelState, ""));
        }

        [Authorize(Roles = "Admin, Elite")]
        [HttpPatch("set-main-wallet/{walletId}")]
        public async Task<IActionResult> SetMainWallet(string userId, string walletId)
        {
            ResponseDto<List<GetWalletDto>> res = new ResponseDto<List<GetWalletDto>>();
            List<GetWalletDto> listOfWallets = new List<GetWalletDto>();

            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to update another user's wallet");
                return BadRequest(Util.BuildResponse<string>(false, "Access denied!", ModelState, ""));
            }
            else if(!await _walletService.HasWallet())
            {
                ModelState.AddModelError("Failed", $"Invalid payload");
                return BadRequest(Util.BuildResponse<string>(false, "You have no wallet!", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    //Wallet wallet = await _walletService.GetMainWallet();
                    if (await _walletService.SetMainWallet(walletId))
                    {
                        List<Wallet> wallets = await _walletService.GetUserWallets();
                        if (wallets.Count > 0)
                        {
                            foreach (var wallet in wallets)
                            {
                                listOfWallets.Add(_mapper.Map<GetWalletDto>(wallet));
                            }
                            res.Message = "Main Wallet set successfully!";
                            res.Status = true;
                            res.Data = listOfWallets;
                            return Ok(res);
                        }

                        return Ok(res);
                    }

                    ModelState.AddModelError("Failed", $"Invalid payload");
                    return BadRequest(Util.BuildResponse<string>(false, "Main wallet not found", ModelState, ""));
                }                
            }

            ModelState.AddModelError("Denied", $"Noob Account can only have one wallet");
            return BadRequest(Util.BuildResponse<string>(false, "Access Denied", ModelState, ""));
        }

        [HttpDelete("delete/{walletId}")]
        public async Task<IActionResult> Delete(string walletId, string userId)
        {
            ResponseDto<GetWalletDto> res = new ResponseDto<GetWalletDto>();
            //check if user logged is the one making the changes - only works for system using Auth tokens
            ClaimsPrincipal currentUser = this.User;
            var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!userId.Equals(currentUserId))
            {
                ModelState.AddModelError("Denied", $"You are not allowed to delete another user's  wallet");
                var result2 = Util.BuildResponse<string>(false, "Access denied!", ModelState, "");
                return BadRequest(result2);
            }

            bool status = await _walletService.Delete(walletId);

            if (status)
            {
                res.Status = true;
                res.Message = "Wallet Deleted";

                return Ok(res);

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to delete wallet", ModelState, ""));
        }
    }
}
