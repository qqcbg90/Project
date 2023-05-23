using System.Web.Mvc;

namespace admin.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return View("Error");
        }

    }
}
