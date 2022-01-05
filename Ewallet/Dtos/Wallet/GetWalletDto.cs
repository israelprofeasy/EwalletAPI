using Ewallet.Dtos.Currency;
using Ewallet.Dtos.WalletCurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Wallet
{
    public class GetWalletDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsMain { get; set; } = false;
        public List<GetWalletCurrencyDto> WalletCurrencies { get; set; } = new List<GetWalletCurrencyDto>();
    }
}
