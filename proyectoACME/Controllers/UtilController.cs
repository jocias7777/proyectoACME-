using System.Web.Mvc;

namespace proyectoACME.Controllers
{
    public class UtilController : Controller
    {
        public ActionResult Hash()
        {
            string hash = BCrypt.Net.BCrypt.HashPassword("Admin123");
            return Content(hash);
        }
    }
}