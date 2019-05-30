using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("authorization", $"Bearer token ->> ")
                };

            FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}UserInfo", encodedParameters).Result;

            return response.StatusCode == HttpStatusCode.OK ? View() : View();
        }
    }
}