using HouseholdManager.Models.Account;
using HouseholdManager.Models.Home;
using Newtonsoft.Json;
using System.Collections.Generic;
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    UserModel result = JsonConvert.DeserializeObject<UserModel>(data);

                    HttpCookie cookie = new HttpCookie("Token", result.access_token);

                    Response.Cookies.Add(cookie);

                    return RedirectToAction(nameof(HomeController.Index), ControllerName);
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.Login), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.Login), formData);
            }
        }

        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies["Token"];

            if (cookie == null)
            {
                return RedirectToAction(nameof(HomeController.Index), ControllerName);
            }

            string token = cookie.Value;

            HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");

            HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}Logout", null).Result;


            if (response.StatusCode == HttpStatusCode.OK)
            {
                Request.Cookies.Remove("Token");

                return RedirectToAction(nameof(HomeController.Index), ControllerName);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), ControllerName);
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(AccountController.Login));
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.Register), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.Register), formData);
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
                //List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
                //{
                //    new KeyValuePair<string, string>("userEmail", formData.Email)
                //};

                //FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = HttpClient.PostAsync($"{ApiUrl}{AccountRoute}ForgotPassword?userEmail={formData.Email}", null).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return View(nameof(AccountController.ResetPassword));
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.ForgotPassword), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.ForgotPassword), formData);
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return View(nameof(AccountController.Login));
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.ResetPassword), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.ResetPassword), formData);
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

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return View(nameof(AccountController.Login));
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.ChangePassword), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.ChangePassword), formData);
            }
        }
    }
}