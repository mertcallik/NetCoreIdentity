using Microsoft.AspNetCore.Identity;

namespace NetIdentityApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }

    }
}
