using Ewallet.Enums;
using System;

namespace Ewallet.Models
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public Status Status { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public string CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
