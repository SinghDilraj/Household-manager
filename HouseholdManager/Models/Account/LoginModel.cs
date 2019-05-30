using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Account
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} is not Valid.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}