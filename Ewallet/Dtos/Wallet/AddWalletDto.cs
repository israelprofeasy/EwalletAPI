using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Wallet
{
    public class AddWalletDto
    {
        [Required]
        public string Name { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
