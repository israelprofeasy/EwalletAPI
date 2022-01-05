using Ewallet.Commons;
using Ewallet.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Data
{
    public class DbInitializer
    {   
        public static async Task Seed(IApplicationBuilder applicationBuilder, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<DataContext>();

                context.Database.EnsureCreated();

                var roles = new string[] { "Noob", "Admin" };

                if (!roleManager.Roles.Any())
                {
                    foreach (var role in roles)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }                

                if (!context.Users.Any())
                {
                    var role = "";
                    User user = new User()
                    {
                        FirstName = "Arsene",
                        LastName = "Merlino",
                        Email = "abideklove@gmail.com",
                        CreatedAt = DateTime.Now
                    };

                    user.UserName = user.Email;

                    role = roles[1];
                    var result = await userManager.CreateAsync(user, "@Merlino07");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

        }

        
        
    }
}
