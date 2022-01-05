using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Fund
{
    public class WithdrawDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string WalletId { get; set; }
        public string CurrencyId { get; set; }
    }
}
