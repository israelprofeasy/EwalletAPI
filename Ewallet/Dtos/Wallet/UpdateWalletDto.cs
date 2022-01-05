using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Wallet
{
    public class UpdateWalletDto
    {
        public string Name { get; set; }
        public bool IsMain { get; set; } = false;
    }
}
