using Ewallet.Commons;
using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Dtos;
using Ewallet.Dtos.User;
using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJWTService _jWTService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IJWTService jWTService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jWTService = jWTService;
        }

        public async Task<ResponseDto<string>> Login(string email, string password, bool rememberMe)
        {
            ResponseDto<string> res = new ResponseDto<string>();

            User existingUser = await _userManager.FindByEmailAsync(email);
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, password);

            if (existingUser == null) res.Status = false;

            else if (!isCorrect) res.Status = false;

            else
            {
                var result = await _signInManager.PasswordSignInAsync(existingUser, password, rememberMe, false);

                if (!result.Succeeded) res.Status = false;

                res.Status = true;

                var jwtToken = await _jWTService.CreateToken(existingUser);
                res.Data = jwtToken;
            }

            return res;
        }
    }
}
