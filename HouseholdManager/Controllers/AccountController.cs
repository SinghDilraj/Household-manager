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
        public ActionResult LoginPage()
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

                    HttpCookie cookie = new HttpCookie("Token", $"bearer {result.Token}");

                    Response.Cookies.Add(cookie);

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.LoginPage), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.LoginPage), formData);
            }
        }

        public ActionResult RegisterPage()
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
                    return RedirectToAction(nameof(AccountController.LoginPage));
                }
                else
                {
                    ModelState.AddModelError("", response.ReasonPhrase);
                    return View(nameof(AccountController.RegisterPage), formData);
                }
            }
            else
            {
                return View(nameof(AccountController.RegisterPage), formData);
            }
        }

        public ActionResult ForgotPasswordPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //public ActionResult Settings()
        //{
        //    return View();
        //}
    }
}