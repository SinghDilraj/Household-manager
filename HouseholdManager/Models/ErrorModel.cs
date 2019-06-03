using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace HouseholdManager.Models
{
    public class ErrorModel
    {
        public string Message { get; set; }
        public string Error { get; set; }
        [JsonProperty("Error_description")]
        public string ErrorDescription { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}