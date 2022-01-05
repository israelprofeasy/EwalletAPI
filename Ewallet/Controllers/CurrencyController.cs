using AutoMapper;
using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Currency;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;

        public CurrencyController(ICurrencyService currencyService, IMapper mapper)
        {
            _currencyService = currencyService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddCurrencyDto model)
        {
            ResponseDto<List<GetCurrencyDto>> res = new ResponseDto<List<GetCurrencyDto>>();

            List<GetCurrencyDto> listOfCurrencies = new List<GetCurrencyDto>();
            if (ModelState.IsValid)
            {

                Currency newCurrency = _mapper.Map<Currency>(model);

                List<Currency> currencies = await _currencyService.Add(newCurrency);

                if (currencies.Count > 0)
                {
                    foreach (var currency in currencies)
                    {
                        listOfCurrencies.Add(_mapper.Map<GetCurrencyDto>(currency));
                    }

                    res.Status = true;
                    res.Message = "Currency added successfully!";
                    res.Data = listOfCurrencies;
                    return Ok(res);
                }
                else
                {
                    ModelState.AddModelError("Failed", $"Invalid payload");
                    return BadRequest(Util.BuildResponse<string>(false, "Unable to add new currency", ModelState, ""));
                }

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to add new currency", ModelState, ""));
        }

        [HttpGet("get-all-currencies")]
        public IActionResult Get()
        {
            ResponseDto<List<GetCurrencyDto>> res = new ResponseDto<List<GetCurrencyDto>>();
            List<GetCurrencyDto> listOfCurrencies = new List<GetCurrencyDto>();

            List<Currency> currencies = _currencyService.GetCurrencies();

            if (currencies.Count > 0)
            {
                foreach (var currency in currencies)
                {
                    listOfCurrencies.Add(_mapper.Map<GetCurrencyDto>(currency));
                }

                res.Status = true;
                res.Message = "List of currencies";
                res.Data = listOfCurrencies;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve currencies", ModelState, ""));
        }

        [HttpGet("get-role-by-id/{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            ResponseDto<GetCurrencyDto> res = new ResponseDto<GetCurrencyDto>();
            GetCurrencyDto model = new GetCurrencyDto();

            Currency currency = await _currencyService.GetCurrencyById(id);

            if (currency != null)
            {
                model = _mapper.Map<GetCurrencyDto>(currency);

                res.Status = true;
                res.Message = "Currency details";
                res.Data = model;
                return Ok(res);
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to retrieve currency", ModelState, ""));
        }

        [HttpPut("update-role/{id}")]
        public async Task<IActionResult> Update(string id, UpdateCurrencyDto model)
        {
            ResponseDto<GetCurrencyDto> res = new ResponseDto<GetCurrencyDto>();

            Currency currency = await _currencyService.GetCurrencyById(id);

            if (currency == null)
            {
                ModelState.AddModelError("Denied", $"Currency does not exist");
                return BadRequest(Util.BuildResponse<string>(false, "Access Denied!", ModelState, ""));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Currency updateCurrency = new Currency
                    {
                        Name = model.Name,
                        ShortCode = model.ShortCode
                    };

                    Currency response = await _currencyService.Update(id, updateCurrency);

                    if (response != null)
                    {
                        GetCurrencyDto updatedCurrency = _mapper.Map<GetCurrencyDto>(response);

                        res.Status = true;
                        res.Message = "Currency updated";
                        res.Data = updatedCurrency;
                    }

                    return Ok(res);
                }
            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to update currency", ModelState, ""));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            ResponseDto<GetCurrencyDto> res = new ResponseDto<GetCurrencyDto>();
            bool status = await _currencyService.Delete(id);

            if (status)
            {
                res.Status = true;
                res.Message = "Currency Deleted";

                return Ok(res);

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to delete currency", ModelState, ""));
        }
    }
}
