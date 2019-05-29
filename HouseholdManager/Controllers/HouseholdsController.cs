using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class HouseholdsController : BaseController
    {
        // GET: Households
        public ActionResult Index()
        {
            return View();
        }
    }
}