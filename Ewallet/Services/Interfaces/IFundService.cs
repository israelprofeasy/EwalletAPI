using Ewallet.Dtos.Fund;
using Ewallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface IFundService
    {
        Task<bool> Fund(FundWalletDto request);
        Task<bool> Withdraw(WithdrawDto request);
    }
}
