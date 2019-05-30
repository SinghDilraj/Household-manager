using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}