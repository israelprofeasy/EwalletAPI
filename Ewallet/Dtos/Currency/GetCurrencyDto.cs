using Ewallet.Dtos.WalletCurrency;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Currency
{
    public class GetCurrencyDto
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter currency name"), MaxLength(10)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter currency short code"), MaxLength(5)]
        public string ShortCode { get; set; }
    }
}
