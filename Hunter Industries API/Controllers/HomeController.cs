using System.Web.Mvc;

namespace HunterIndustriesAPI.Controllers
{
    /// <summary>
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// </summary>
        public ActionResult Index()
        {
            return Redirect("/swagger/ui/index");
        }
    }
}
