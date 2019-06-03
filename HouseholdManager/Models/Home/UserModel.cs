using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HouseholdManager.Models.Home
{
    public class UserModel
    {
        public string UserName { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}