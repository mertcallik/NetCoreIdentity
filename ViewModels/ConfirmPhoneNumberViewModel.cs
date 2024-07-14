using System.ComponentModel.DataAnnotations;

namespace NetIdentityApp
{
    public class ConfirmPhoneNumberViewModel
    {
        [Required]
        public string? UserId { get; set; }
        [Required]
        public string? Token { get; set; }

        public string? TokenFromUser { get; set; } = "";
    }
}