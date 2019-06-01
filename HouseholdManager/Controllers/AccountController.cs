using HouseholdManager.Models;
using HouseholdManager.Models.Account;
using HouseholdManager.Models.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel formData)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", formData.Email),
                    new KeyValuePair<string, string>("password", formData.Password),
                    new KeyValuePair<string, string>("grant_type", "password")
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{TokenRoute}", encodedParameters).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    UserModel result = JsonConvert.DeserializeObject<UserModel>(data);

                    HttpCookie TokenCookie = new HttpCookie("Token", result.access_token);

                    Response.Cookies.Add(TokenCookie);

                    HttpCookie UserNameCookie = new HttpCookie("UserName", result.UserName);

                    Response.Cookies.Add(UserNameCookie);

                    return RedirectToAction(nameof(HomeController.Index), Home);
                }
                else
                {
                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError("", errorModel?.Error_description);
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
                return View(formData);
            }
        }

        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["Token"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(AccountController.Login));
            }

            string token = cookie.Value;

            HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}Logout", null).Result;

            HttpCookie TokenResetCookie = new HttpCookie("Token", null);

            TokenResetCookie.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(TokenResetCookie);

            HttpCookie UserNameResetCookie = new HttpCookie("UserName", null);

            TokenResetCookie.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(UserNameResetCookie);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), Home);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel formData)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", formData.Email),
                    new KeyValuePair<string, string>("password", formData.Password),
                    new KeyValuePair<string, string>("confirmPassword", formData.ConfirmPassword)
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}Register", encodedParameters).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(AccountController.Login));
                }
                else
                {
                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        if (errorModel != null)
                        {
                            if (errorModel.ModelState != null)
                            {
                                if (errorModel.ModelState.Values.Any())
                                {
                                    foreach (string[] value in errorModel.ModelState.Values)
                                    {
                                        if (value.Any())
                                        {
                                            foreach (string val in value)
                                            {
                                                ModelState.AddModelError("", val);
                                            }
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
                return View(formData);
            }
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel formData)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}ForgotPassword?userEmail={formData.Email}", null).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return View(nameof(AccountController.ResetPassword));
                }
                else
                {
                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        if (errorModel != null)
                        {
                            if (errorModel.ModelState != null)
                            {
                                if (errorModel.ModelState.Values.Any())
                                {
                                    foreach (string[] value in errorModel.ModelState.Values)
                                    {
                                        if (value.Any())
                                        {
                                            foreach (string val in value)
                                            {
                                                ModelState.AddModelError("", val);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError("", response.ReasonPhrase);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email does not exist. Please try again.");
                    }

                    return View(formData);
                }
            }
            else
            {
                return View(formData);
            }
        }


        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel formData)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Code", formData.Code),
                    new KeyValuePair<string, string>("Email", formData.Email),
                    new KeyValuePair<string, string>("NewPassword", formData.Password),
                    new KeyValuePair<string, string>("ConfirmPassword", formData.ConfirmPassword)
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}ResetPassword", encodedParameters).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return View(nameof(AccountController.Login));
                }
                else
                {
                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError("", errorModel?.Message);
                        ModelState.AddModelError("", errorModel?.Error_description);

                        if (errorModel != null)
                        {
                            if (errorModel.ModelState != null)
                            {
                                if (errorModel.ModelState.Values.Any())
                                {
                                    foreach (string[] value in errorModel.ModelState.Values)
                                    {
                                        if (value.Any())
                                        {
                                            foreach (string val in value)
                                            {
                                                ModelState.AddModelError("", val);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError("", "Code not valid. Please try again.");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        ModelState.AddModelError("", "Email Address does not exist. Please try again.");
                    }

                    return View(formData);
                }
            }
            else
            {
                return View(formData);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel formData)
        {
            if (ModelState.IsValid)
            {
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("OldPassword", formData.OldPassword),
                    new KeyValuePair<string, string>("NewPassword", formData.NewPassword),
                    new KeyValuePair<string, string>("ConfirmPassword", formData.ConfirmPassword)
                };

                FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpCookie cookie = Request.Cookies["Token"];

                if (cookie == null)
                {
                    return RedirectToAction(nameof(AccountController.Login));
                }

                string token = cookie.Value;

                HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}ChangePassword", encodedParameters).Result;

                string data = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(AccountController.Logout), "Account", null);
                }
                else
                {
                    ErrorModel errorModel = JsonConvert.DeserializeObject<ErrorModel>(data);

                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        if (errorModel.ModelState.Values.Any())
                        {
                            foreach (string[] value in errorModel.ModelState.Values)
                            {
                                if (value.Any())
                                {
                                    foreach (string val in value)
                                    {
                                        ModelState.AddModelError("", val);
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
                return View(formData);
            }
        }
    }
}