using HouseholdManager.Models;
using HouseholdManager.Models.Households;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                HouseholdModel viewModel = JsonConvert.DeserializeObject<HouseholdModel>(data);

                return View(viewModel);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }
        }

        [HttpGet]
        public ActionResult GetAllHouseholds()
        {
            HttpCookie cookie = Request.Cookies["Token"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            string token = cookie.Value;

            HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}").Result;

            string data = response.Content.ReadAsStringAsync().Result;

            List<HouseholdModel> viewModels = JsonConvert.DeserializeObject<List<HouseholdModel>>(data);

            return View(viewModels);
        }

        [HttpGet]
        public ActionResult Create()
        {
            HttpCookie cookie = Request.Cookies["Token"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(HouseholdModel formData)
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
                    HouseholdModel viewModel = JsonConvert.DeserializeObject<HouseholdModel>(data);

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

        [HttpGet]
        public ActionResult Edit(int? id)
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

                HouseholdModel viewModel = JsonConvert.DeserializeObject<HouseholdModel>(data);

                return View(viewModel);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }
        }

        [HttpPost]
        public ActionResult Edit(HouseholdModel formData)
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

                HttpResponseMessage response = HttpClient.PutAsync($"{ApiUrl}{HouseholdRoute}/{formData.Id}", encodedParameters).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    HouseholdModel viewModel = JsonConvert.DeserializeObject<HouseholdModel>(data);

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

        public ActionResult Delete(int? id)
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

                HttpResponseMessage response = HttpClient.DeleteAsync($"{ApiUrl}{HouseholdRoute}/{id}").Result;

                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds));
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }
        }

        [HttpGet]
        public ActionResult InviteUser(int? id)
        {
            HttpCookie cookie = Request.Cookies["Token"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            if (id.HasValue)
            {
                InviteUserModel model = new InviteUserModel
                {
                    HouseholdId = id.Value
                };

                return View(model);
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
            }
        }


        [HttpPost]
        public ActionResult InviteUser(InviteUserModel formData)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    ModelState.AddModelError("", "Unauthorized request.");
                    return View(formData);
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{HouseholdRoute}/{formData.Email}/{formData.HouseholdId}", null).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id = formData.HouseholdId });
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError("", "Unauthorized request.");

                    return View(formData);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (errorModel != null)
                    {
                        if (errorModel.ModelState != null)
                        {
                            foreach (KeyValuePair<string, string[]> pair in errorModel.ModelState)
                            {
                                if (pair.Value.Any())
                                {
                                    foreach (string val in pair.Value)
                                    {
                                        ModelState.AddModelError(pair.Key, val);
                                    }
                                }
                            }
                        }
                    }

                    return View(formData);
                }
                else
                {
                    ModelState.AddModelError("", "Email address not valid. User not found.");

                    return View(formData);
                }
            }
            else
            {
                return View(formData);
            }
        }

        public ActionResult Join(int? id)
        {
            if (id.HasValue)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    //ModelState.AddModelError("", "Unauthorized request.");
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{HouseholdRoute}/Join/{id}", null).Result;

                return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
            }
        }

        public ActionResult Leave(int? id)
        {
            if (id.HasValue)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    //ModelState.AddModelError("", "Unauthorized request.");
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{HouseholdRoute}/Leave/{id}", null).Result;

                return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetHousehold), new { id });
            }
        }
    }
}
