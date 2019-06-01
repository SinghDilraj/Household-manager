using System;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int HouseholdId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}