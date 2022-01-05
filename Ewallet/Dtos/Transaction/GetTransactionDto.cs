using System;

namespace Ewallet.Dtos.Transaction
{
    public class GetTransactionDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string WalletId { get; set; }
        public string CurrencyId { get; set; }
    }
}