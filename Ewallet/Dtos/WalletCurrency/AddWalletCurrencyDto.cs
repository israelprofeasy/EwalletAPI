using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.WalletCurrency
{
    public class AddWalletCurrencyDto
    {
        public bool IsMain { get; set; } = false;
        public string WalletId { get; set; }
        public string CurrencyId { get; set; }
    }
}
