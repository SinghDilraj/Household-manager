﻿using System;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Households
{
    public class HouseholdModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string OwnerEmail { get; set; }
    }
}