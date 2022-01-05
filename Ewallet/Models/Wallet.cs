using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Models
{
    public class Wallet : BaseEntity
    {
        [Required(ErrorMessage = "Please enter wallet name"), MaxLength(30)]
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsMain { get; set; } = false;
        public string UserId { get; set; }
        public User User { get; set; }
        public List<WalletCurrency> WalletCurrencies { get; set; }
        public List<Transaction> Transactions { get; set; }

        public Wallet()
        {
            WalletCurrencies = new List<WalletCurrency>();
            Transactions = new List<Transaction>();
        }
    }
}
