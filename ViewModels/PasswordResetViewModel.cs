using System.ComponentModel.DataAnnotations;

namespace NetIdentityApp
{
    public class PasswordResetViewModel
    {
        [Required]
        public string? Code { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]


        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? PasswordConfirm { get; set; }
    }
}