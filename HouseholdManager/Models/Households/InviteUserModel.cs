using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Households
{
    public class InviteUserModel
    {
        [Required]
        public int HouseholdId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}