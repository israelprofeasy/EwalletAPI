using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.User
{
    public class GetUserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        //public string LastName { get; set; }        
        public string Email { get; set; }
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public GetUserDto()
        {
            Roles = new List<string>();
        }
    }
}
