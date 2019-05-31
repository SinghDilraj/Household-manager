using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}