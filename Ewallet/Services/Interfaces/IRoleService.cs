using Ewallet.Dtos;
using Ewallet.Dtos.Role;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<IdentityRole>> Add(IdentityRole newRole);
        List<IdentityRole> GetRoles();
        Task<IdentityRole> GetRoleById(string id);
        Task<IdentityRole> Update(string id, IdentityRole role);
        Task<bool> Delete(string id);
    }
}
