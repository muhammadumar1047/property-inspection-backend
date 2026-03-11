using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PropertyInspection.Core.Entities;
using PropertyInspection.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Data.Seeder
{
    public class IdentitySeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<AppDbContext>();

            string adminEmail = "superadmin@grc.com";
            string adminPassword = "abcD@123";

            var identityUser = await userManager.FindByEmailAsync(adminEmail);

            if (identityUser == null)
            {
                identityUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    AgencyId = null
                };
                await userManager.CreateAsync(identityUser, adminPassword);
            }

            var domainUser = await db.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUser.Id);
            if (domainUser == null)
            {
                domainUser = new User
                {
                    IdentityUserId = identityUser.Id,

                    FirstName = "Super",
                    LastName = "Admin",
                    Email = adminEmail,
                    AgencyId = null,
                    IsSuperAdmin = true,
                    IsActive = true
                };
                db.Users.Add(domainUser);
                await db.SaveChangesAsync();
            }

            Console.WriteLine("SuperAdmin seeded successfully!");
        }
    }
}
