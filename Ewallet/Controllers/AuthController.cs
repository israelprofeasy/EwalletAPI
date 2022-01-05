using Ewallet.Commons;
using Ewallet.Dtos;
using Ewallet.Dtos.Auth;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthController(IAuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("Invalid", "Credentials provided by the user is invalid");
                    return BadRequest(Util.BuildResponse<object>(false, "Invalid credentials", ModelState, null));
                }

                // check if user's email is confirmed
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    ResponseDto<string> res = await _authService.Login(model.Email, model.Password, model.RememberMe);

                    if (!res.Status)
                    {
                        ModelState.AddModelError("Invalid", "Credentials provided by the user is invalid");
                        return BadRequest(Util.BuildResponse<object>(false, "Invalid credentials", ModelState, null));
                    }

                    var userData = res.Data;

                    return Ok(Util.BuildResponse(true, "Login is sucessful!", null, userData));
                }

            }

            ModelState.AddModelError("Failed", $"Invalid payload");
            return BadRequest(Util.BuildResponse<string>(false, "Unable to login", ModelState, ""));
        }
    }
}
