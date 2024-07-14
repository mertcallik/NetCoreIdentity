using System.ComponentModel.DataAnnotations;

namespace NetIdentityApp.ViewModels
{
    public class UserUpdateViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }

        [EmailAddress]
        [RegularExpression(pattern: @"^[a-zA-Z0-9._%+-]+@(gmail\.com)$", ErrorMessage = "Sadece @gmail.com ile biten e-posta adresleri kabul edilir.")]
        public string? Email { get; set; }



        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Parolanız minimum {1} maximum {2} karakterden oluşmalıdır")]
        public string? Password { get; set; }


        [Compare(nameof(Password), ErrorMessage = "Parolalar eşleşmiyor.")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }

        public IList<string>? SelectedRoles { get; set; } = new List<string>();
    }
}
