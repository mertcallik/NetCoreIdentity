using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetIdentityApp.Models;

namespace NetIdentityApp.Data.Concreate.EfCore
{
    public static class SeedData
    {
        private const string AdminUser = "admin";
        private const string AdminPassword = "Admin_1";
        public static async void SeedDb(IApplicationBuilder app)
        {

            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }

                var userManager = app.ApplicationServices.CreateScope().ServiceProvider
                    .GetRequiredService<UserManager<AppUser>>();
                
                var user = await userManager.FindByNameAsync(AdminUser);
                if (user==null)
                {
                    user = new AppUser()
                    {
                        PhoneNumber = "444 4 444",
                        Email = "Admin@gmail.com",
                        UserName = AdminUser,
                        FullName= "Admin Calik",

                    };
                   await userManager.CreateAsync(user, AdminPassword);

                }



        }
    }
}
