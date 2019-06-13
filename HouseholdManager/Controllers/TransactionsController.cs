using HouseholdManager.Models;
using HouseholdManager.Models.Category;
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
        public ActionResult View(int? id, bool isOwner)
        {
            ViewBag.IsOwner = isOwner;
            return View();
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

                ViewBag.Categories = categories;

                return View();
            }
            else
            {
                return RedirectToAction(nameof(BankAccountsController.View), "BankAccounts", new { id = bankAccountId, isOwner = true });
            }
        }

        [HttpPost]
        public ActionResult Create(TransactionModel formData)
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

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{BankAccountRoute}", encodedParameters).Result;

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
                return View(ModelState);
            }
        }
    }
}