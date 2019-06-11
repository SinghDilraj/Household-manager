using HouseholdManager.Models.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Models.BankAccounts
{
    public class BankAccountModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public decimal Balance { get; set; }
        [Required]
        public int HouseholdId { get; set; }
        public List<TransactionModel> Transactions { get; set; }

        public BankAccountModel()
        {
            Transactions = new List<TransactionModel>();
        }
    }
}