using Ewallet.Data.Repositories.Interfaces;
using Ewallet.Dtos;
using Ewallet.Dtos.Role;
using Ewallet.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<List<IdentityRole>> Add(IdentityRole newRole)
        {
            try
            {
                await _roleRepository.Add(newRole);
                var result = _roleRepository.GetAllRoles();
                return result;                
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> Delete(string id)
        {
            bool status = false;

            try
            {
                if (await _roleRepository.Delete<string>(id))
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                // log err
                throw new Exception(ex.Message);
            }

            return status;
        }

        public async Task<IdentityRole> GetRoleById(string id)
        {
            IdentityRole role = null;
            try
            {
                role = await _roleRepository.GetRoleById(id);
            }
            catch (Exception)
            {

                throw;
            }
            return role;
        }

        public List<IdentityRole> GetRoles()
        {
            try
            {
                var result = _roleRepository.GetAllRoles();
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IdentityRole> Update(string id, IdentityRole role)
        {
            IdentityRole updatedRole = null;

            try
            {
                IdentityRole getRole = await _roleRepository.GetRoleById(id);
                if (getRole != null)
                {
                    getRole.Name = role.Name;

                }
                if (await _roleRepository.Edit<string>(id))
                {
                    updatedRole = new IdentityRole
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return updatedRole;
        }
    }
}
