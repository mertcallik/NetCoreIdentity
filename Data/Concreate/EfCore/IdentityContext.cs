using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetIdentityApp.Models;

namespace NetIdentityApp.Data.Concreate.EfCore
{
    public class IdentityContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options):base(options)
        {
            
        }
    }
}
