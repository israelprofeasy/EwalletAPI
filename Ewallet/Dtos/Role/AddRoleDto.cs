using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Role
{
    public class AddRoleDto
    {
        [Required]
        public string Name { get; set; }
    }
}
