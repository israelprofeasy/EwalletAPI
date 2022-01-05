using Ewallet.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        public RoleRepository(RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<bool> Add<T>(T entity)
        {
            IdentityRole role = entity as IdentityRole;
            try
            {
                if(await _roleManager.CreateAsync(role) != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return false;
        }

        public async Task<bool> Delete<T>(T entity)
        {
            string id = entity as string;

            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role != null)
                {
                    IdentityResult result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return false;
        }

        public async Task<bool> Edit<T>(T entity)
        {
            string id = entity as string;

            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role != null)
                {
                    IdentityResult result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

            return false;
        }

        public List<IdentityRole> GetAllRoles()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IdentityRole> GetRoleById(string id)
        {
            IdentityRole role = null;
            try
            {
                role = await _roleManager.FindByIdAsync(id);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return role;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
