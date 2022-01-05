using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.WalletCurrency
{
    public class GetWalletCurrencyDto
    {
        public string CurrencyId { get; set; }
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public bool IsMain { get; set; } = false;
        public decimal Balance { get; set; }
    }
}
