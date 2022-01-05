using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Please enter first name"), MaxLength(30)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name"), MaxLength(30)]
        public string LastName { get; set; }

        public List<Wallet> Wallets { get; set; }
        public List<Photo> Photos { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public User()
        {
            Wallets = new List<Wallet>();
            Photos = new List<Photo>();
        }
    }
}
