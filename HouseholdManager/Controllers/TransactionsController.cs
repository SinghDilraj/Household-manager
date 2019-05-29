using System.Web.Mvc;

namespace HouseholdManager.Controllers
{
    public class TransactionsController : BaseController
    {
        // GET: Transactions
        public ActionResult Index()
        {
            return View();
        }
    }
}