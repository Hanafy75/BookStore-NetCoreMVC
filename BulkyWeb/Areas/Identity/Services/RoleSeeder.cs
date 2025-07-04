using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace BulkyWeb.Areas.Identity.Services
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<AppDbContext>();
            //migrations if they are not applied
            try
            {
                if ((await context.Database.GetPendingMigrationsAsync()).Count() > 0)
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error instead of swallowing it
                Console.WriteLine($"Migration failed: {ex.Message}");
                throw; // Re-throw to see the actual error
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            string[] roles = { Roles.Admin, Roles.Customer, Roles.Employee, Roles.Company };

            foreach(var role in roles)
            {
                if(! await roleManager!.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }


            //create admin user if not exists
            string adminEmail = "admin@exampl.com";
            string adminPassword = "Admin@123";

            var adminUser = await userManager!.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = new MailAddress(adminEmail).User,
                    Email = adminEmail,
                    Name = "Mostafa"
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                //assign to admin role
                if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                else throw new Exception("Failed to create Admin user: ");
            }
            else
            {
                //check if the user in Admin role?
                if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }
        }
    }
}
