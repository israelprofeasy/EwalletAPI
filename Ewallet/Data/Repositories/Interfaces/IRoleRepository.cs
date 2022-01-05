using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Interfaces
{
    public interface IRoleRepository : ICRUDRepository
    {
        List<IdentityRole> GetAllRoles();
        Task<IdentityRole> GetRoleById(string id);
    }
}
