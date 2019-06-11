using HouseholdManager.Models.Category;
using System;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.Transactions
{
    public class TransactionModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Initiated { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public CategoryModel Category { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BankAccountId { get; set; }
        public bool IsVoid { get; set; }
    }
}