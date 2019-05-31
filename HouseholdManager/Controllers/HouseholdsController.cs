using HouseholdManager.Models.Households;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class HouseholdsController : BaseController
    {

        [HttpGet]
        public ActionResult GetHousehold(int? id)
        {
            if (id.HasValue)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}/{id}").Result;

                string data = response.Content.ReadAsStringAsync().Result;

                HouseholdViewModel viewModel = JsonConvert.DeserializeObject<HouseholdViewModel>(data);

                return View(viewModel);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(HouseholdViewModel formData)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Name", formData.Name),
                    new KeyValuePair<string, string>("Description", formData.Description)
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{HouseholdRoute}", encodedParameters).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    HouseholdViewModel viewModel = JsonConvert.DeserializeObject<HouseholdViewModel>(data);

                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id = viewModel.Id });
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);

                    return View(formData);
                }
            }
            else
            {
                return View(formData);
            }
        }
    }
}