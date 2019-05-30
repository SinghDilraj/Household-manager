using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Account
{
    public class ChangePasswordModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "The NewPassword and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}