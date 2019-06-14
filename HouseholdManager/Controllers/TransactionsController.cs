using HouseholdManager.Models;
using HouseholdManager.Models.Category;
using HouseholdManager.Models.Households;
using HouseholdManager.Models.Transactions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class TransactionsController : BaseController
    {
        [HttpGet]
        public ActionResult View(int? id)
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

                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{TransactionRoute}{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    TransactionModel model = JsonConvert.DeserializeObject<TransactionModel>(data);

                    HttpResponseMessage Usersresponse = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}Users/{model.Category.HouseholdId}").Result;

                    string UsersData = Usersresponse.Content.ReadAsStringAsync().Result;

                    HouseholdMembersModel UsersModel = JsonConvert.DeserializeObject<HouseholdMembersModel>(UsersData);

                    HttpCookie UserNameCookie = Request.Cookies["UserName"];

                    if (UserNameCookie == null)
                    {
                        return RedirectToAction(nameof(AccountController.Login), "Account");
                    }

                    string UserNameToken = UserNameCookie.Value;

                    if (UsersModel.Members.Any(m => m.Email == UserNameToken) || UsersModel.Owner.Email == UserNameToken)
                    {
                        ViewBag.IsAllowed = true;
                    }
                    else
                    {
                        ViewBag.IsAllowed = false;
                    }

                    return View(model);
                }
                else
                {
                    return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
                }
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
            }
        }

        [HttpGet]
        public ActionResult Create(int? householdId, int? bankAccountId)
        {
            if (householdId.HasValue)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}Categories/{householdId}").Result;

                string data = response.Content.ReadAsStringAsync().Result;

                List<CategoryModel> model = JsonConvert.DeserializeObject<List<CategoryModel>>(data);

                SelectList categories = new SelectList(model, "Id", "Name");

                ViewData["Categories"] = categories;

                ViewBag.HouseholdId = householdId;

                return View();
            }
            else
            {
                return RedirectToAction(nameof(BankAccountsController.View), "BankAccounts", new { id = bankAccountId, isOwner = true });
            }
        }

        [HttpPost]
        public ActionResult Create(TransactionModel formData, int householdId)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", formData.Title),
                    new KeyValuePair<string, string>("Description", formData.Description),
                    new KeyValuePair<string, string>("Amount", formData.Amount.ToString()),
                    new KeyValuePair<string, string>("Initiated", formData.Initiated.ToString()),
                    new KeyValuePair<string, string>("CategoryId", formData.CategoryId.ToString()),
                    new KeyValuePair<string, string>("BankAccountId", formData.BankAccountId.ToString())
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{TransactionRoute}", encodedParameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(BankAccountsController.View), "BankAccounts", new { id = formData.BankAccountId, isOwner = true });
                }
                else
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
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
                    }
                    else
                    {
                        ModelState.AddModelError("", response.ReasonPhrase);
                    }

                    return View(formData);
                }
            }
            else
            {
                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}Categories/{householdId}").Result;

                string data = response.Content.ReadAsStringAsync().Result;

                List<CategoryModel> model = JsonConvert.DeserializeObject<List<CategoryModel>>(data);

                SelectList categories = new SelectList(model, "Id", "Name");

                ViewData["Categories"] = categories;

                return View(ModelState);
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

                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{TransactionRoute}{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    TransactionModel model = JsonConvert.DeserializeObject<TransactionModel>(data);

                    HttpResponseMessage categoriesResponse = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}Categories/{model.Category.HouseholdId}").Result;

                    string catData = categoriesResponse.Content.ReadAsStringAsync().Result;

                    List<CategoryModel> viewModel = JsonConvert.DeserializeObject<List<CategoryModel>>(catData);

                    SelectList categories = new SelectList(viewModel, "Id", "Name");

                    ViewData["Categories"] = categories;

                    ViewBag.HouseholdId = model.Category.HouseholdId;

                    return View(model);
                }
                else
                {
                    return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
                }
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
            }
        }

        [HttpPost]
        public ActionResult Edit(TransactionModel formData, int householdId)
        {
            if (ModelState.IsValid)
            {
                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Title", formData.Title),
                    new KeyValuePair<string, string>("Description", formData.Description),
                    new KeyValuePair<string, string>("Amount", formData.Amount.ToString()),
                    new KeyValuePair<string, string>("Initiated", formData.Initiated.ToString()),
                    new KeyValuePair<string, string>("CategoryId", formData.CategoryId.ToString()),
                    new KeyValuePair<string, string>("BankAccountId", formData.BankAccountId.ToString())
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PutAsync($"{ApiUrl}{TransactionRoute}{formData.Id}", encodedParameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households", new { id = householdId });
                }
                else
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
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
                    }
                    else
                    {
                        ModelState.AddModelError("", response.ReasonPhrase);
                    }

                    HttpResponseMessage responsea = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}Categories/{householdId}").Result;

                    string dataa = responsea.Content.ReadAsStringAsync().Result;

                    List<CategoryModel> modela = JsonConvert.DeserializeObject<List<CategoryModel>>(dataa);

                    SelectList categories = new SelectList(modela, "Id", "Name");

                    ViewData["Categories"] = categories;

                    return View(formData);
                }
            }
            else
            {
                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{HouseholdRoute}Categories/{householdId}").Result;

                string data = response.Content.ReadAsStringAsync().Result;

                List<CategoryModel> model = JsonConvert.DeserializeObject<List<CategoryModel>>(data);

                SelectList categories = new SelectList(model, "Id", "Name");

                ViewData["Categories"] = categories;

                return View(ModelState);
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

                HttpResponseMessage modelResponse = HttpClient.GetAsync($"{ApiUrl}{TransactionRoute}{id}").Result;

                string data = modelResponse.Content.ReadAsStringAsync().Result;

                TransactionModel model = JsonConvert.DeserializeObject<TransactionModel>(data);

                HttpResponseMessage response = HttpClient.DeleteAsync($"{ApiUrl}{TransactionRoute}{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households", new { id = model.Category.HouseholdId });
                }
                else
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = model.Category.HouseholdId });
                }
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
            }
        }

        public ActionResult Void(int? id)
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

                HttpResponseMessage modelResponse = HttpClient.GetAsync($"{ApiUrl}{TransactionRoute}{id}").Result;

                string data = modelResponse.Content.ReadAsStringAsync().Result;

                TransactionModel model = JsonConvert.DeserializeObject<TransactionModel>(data);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{TransactionRoute}Void/{id}?Void=true", null).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households", new { id = model.Category.HouseholdId });
                }
                else
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = model.Category.HouseholdId });
                }
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
            }
        }
    }
}