using System;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Households
{
    public class HouseholdViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        //public UserViewModel Owner { get; set; }
        //public List<UserViewModel> Members { get; set; }
        //public List<CategoryViewModel> Categories { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}