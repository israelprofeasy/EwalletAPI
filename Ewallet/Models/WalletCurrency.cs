using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Models
{
    public class WalletCurrency : BaseEntity
    {
        public decimal Balance { get; set; }
        public bool IsMain { get; set; } = false;
        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public string CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
