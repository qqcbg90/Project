using System.Web.Mvc;

namespace web.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return View("Error");
        }

    }
}
