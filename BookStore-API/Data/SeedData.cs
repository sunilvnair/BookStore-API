using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public static class SeedData
    {
        public async static Task seed (UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await seedRoles(roleManager);
            await seedUsers(userManager);

        }
        private async static Task seedUsers(UserManager<IdentityUser> userManager)
        {
           if (await userManager.FindByEmailAsync("admin@myBookStore.com")==null)
            {
                var user = new IdentityUser
                {
                    UserName = "Admin",
                    Email = "admin@myBookStore.com"
                };
                var result = await userManager.CreateAsync(user,"P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "SysAdmin");
                }
            }
            if (await userManager.FindByEmailAsync("sunilvnair@gmail.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "sunilvnair",
                    Email = "sunilvnair@gmail.com"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "SysAdmin");
                }
            }

            if (await userManager.FindByEmailAsync("sunair@qf.org.qa") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "sunair",
                    Email = "sunair@qf.org.qa"
                };
                var result = await userManager.CreateAsync(user, "P@ssword1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }
        public async static Task  seedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("SysAdmin"))
            {
                var role = new IdentityRole
                {
                    Name = "SysAdmin"
                };
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole
                {
                    Name = "Customer"
                };
                await roleManager.CreateAsync(role);
            }


        }
    }
}
