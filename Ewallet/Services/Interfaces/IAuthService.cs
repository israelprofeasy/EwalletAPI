using Ewallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<string>> Login(string email, string password, bool rememberMe);
    }
}
