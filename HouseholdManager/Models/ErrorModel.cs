using System.Collections.Generic;

namespace HouseholdManager.Models
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public string Error { get; set; }
        public string Error_description { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}