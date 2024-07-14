using System.ComponentModel.DataAnnotations;

namespace NetIdentityApp.ViewModels
{
    public class UserCreateViewModel
    {
        [Required]
        public string? UserName { get; set; }
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }



        [Required(ErrorMessage = "Parola alanı zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Parolanız minimum {1} maximum {2} karakterden oluşmalıdır")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Parola doğrulama alanı zorunludur")]


        [Compare(nameof(Password), ErrorMessage = "Parolalar eşleşmiyor.")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }
    }
}
