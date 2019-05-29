using System.Net.Http;
using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class BaseController : Controller
    {
        protected const string ApiUrl = "http://localhost:53370/";
        protected const string HouseholdRoute = "Api/Household/";
        protected const string CategoryRoute = "Api/Category/";
        protected const string BankAccountRoute = "Api/BankAccount/";
        protected const string TransactionRoute = "Api/Transaction/";

        protected HttpClient HttpClient = new HttpClient();
    }
}