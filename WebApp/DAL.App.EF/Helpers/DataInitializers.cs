using System;
using Domain.App.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.EF.Helpers
{
    public class DataInitializers
    {
        public static void MigrateDatabase(AppDbContext context)
        {
            context.Database.Migrate();
        }

        public static bool DeleteDatabase(AppDbContext context)
        {
            return context.Database.EnsureDeleted();
        }

        public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {

            var roleNames = new string[] {"User", "Admin"};
            foreach (var roleName in roleNames)
            {
                var role = roleManager.FindByNameAsync(roleName).Result;
                if (role == null)
                {
                    role = new AppRole();
                    role.Name = roleName;
                    var result = roleManager.CreateAsync(role).Result;
                    if (!result.Succeeded)
                    {
                        throw new ApplicationException("Role creation failed!");
                    }
                }

            }


            const string userName = "" +
                                    "admin@admin.com" +
                                    "";
            const string passWord = "Admin.Admin.2020";

            var user = userManager.FindByNameAsync(userName).Result;
            if (user == null)
            {
                user = new AppUser();
                user.Email = "admin@admin.com";
                user.UserName = "admin@admin.com";
                user.Name = "Admin";
                user.Gender = "Apache helicopter";
                user.AvatarImg =
                    "https://otsukai.com/optimized?key=public/item/25505/original-5de6bd1668224.jpeg&operation=resize&w=960";

                var result = userManager.CreateAsync(user, passWord).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException("User creation failed!");

                }

                var roleResult = userManager.AddToRoleAsync(user, "Admin").Result;
                roleResult = userManager.AddToRoleAsync(user, "User").Result;

            }


        }

        public static void SeedData(AppDbContext context)
        {

        }
    }
    
}