using System.ComponentModel.DataAnnotations;

namespace NetIdentityApp.ViewModels
{
    public class PasswordForgotViewModel
    {
        [Required]
        public string? Email { get; set; }
    }
}
