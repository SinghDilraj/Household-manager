using HouseholdManager.Models.Account;
using System.Collections.Generic;

namespace HouseholdManager.Models.Households
{
    public class HouseholdMembersModel
    {
        public int HouseholdId { get; set; }
        public UserViewModel Owner { get; set; }
        public List<UserViewModel> Members { get; set; }
        public List<UserViewModel> Invitees { get; set; }

        public HouseholdMembersModel()
        {
            Members = new List<UserViewModel>();
            Invitees = new List<UserViewModel>();
        }
    }
}