﻿using HouseholdManager.Models;
using HouseholdManager.Models.BankAccounts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class BankAccountsController : BaseController
    {
        [HttpGet]
        public ActionResult View(int? id, bool isOwner)
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

                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{BankAccountRoute}{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    BankAccountModel model = JsonConvert.DeserializeObject<BankAccountModel>(data);

                    ViewBag.IsOwner = isOwner;

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
        public ActionResult Create(int? HouseholdId)
        {
            if (HouseholdId.HasValue)
            {
                BankAccountModel model = new BankAccountModel
                {
                    HouseholdId = HouseholdId.Value,
                };

                return View(model);
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
            }
        }

        [HttpPost]
        public ActionResult Create(BankAccountModel formData)
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
                    new KeyValuePair<string, string>("Name", formData.Name),
                    new KeyValuePair<string, string>("Description", formData.Description),
                    new KeyValuePair<string, string>("HouseholdId", formData.HouseholdId.ToString())
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{BankAccountRoute}", encodedParameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = formData.HouseholdId });
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

                HttpResponseMessage response = HttpClient.GetAsync($"{ApiUrl}{BankAccountRoute}{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    BankAccountModel model = JsonConvert.DeserializeObject<BankAccountModel>(data);

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
        public ActionResult Edit(BankAccountModel formData)
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
                    new KeyValuePair<string, string>("Name", formData.Name),
                    new KeyValuePair<string, string>("Description", formData.Description),
                    new KeyValuePair<string, string>("HouseholdId", formData.HouseholdId.ToString())
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PutAsync($"{ApiUrl}{BankAccountRoute}{formData.Id}", encodedParameters).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = formData.HouseholdId });
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

        public ActionResult UpdateBalance(int? id)
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

                HttpResponseMessage modelResponse = HttpClient.GetAsync($"{ApiUrl}{BankAccountRoute}{id}").Result;

                string data = modelResponse.Content.ReadAsStringAsync().Result;

                BankAccountModel model = JsonConvert.DeserializeObject<BankAccountModel>(data);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{BankAccountRoute}UpdateBalance/{id}", null).Result;

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = model.HouseholdId });
                }
                else
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = model.HouseholdId });
                }
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
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

                HttpResponseMessage modelResponse = HttpClient.GetAsync($"{ApiUrl}{BankAccountRoute}{id}").Result;

                string data = modelResponse.Content.ReadAsStringAsync().Result;

                BankAccountModel model = JsonConvert.DeserializeObject<BankAccountModel>(data);

                HttpResponseMessage response = HttpClient.DeleteAsync($"{ApiUrl}{BankAccountRoute}{id}").Result;

                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = model.HouseholdId });
                }
                else
                {
                    return RedirectToAction(nameof(HouseholdsController.GetHousehold), "Households", new { id = model.HouseholdId });
                }
            }
            else
            {
                return RedirectToAction(nameof(HouseholdsController.GetAllHouseholds), "Households");
            }
        }
    }
}