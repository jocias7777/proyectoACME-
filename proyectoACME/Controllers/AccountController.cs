using System.Web.Mvc;
using proyectoACME.DAL;
using proyectoACME.Models.ViewModels;
using proyectoACME.Services;

namespace proyectoACME.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (AuthService.EstaLogueado(Session))
                return RedirectToAction("Index", "Encuestas");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                model.Error = "Usuario o contraseña incorrectos";
                return View(model);
            }

            var usuario = UsuarioDAL.ObtenerPorUsername(model.Username);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash))
            {
                model.Error = "Usuario o contraseña incorrectos";
                return View(model);
            }

            Session["UsuarioId"] = usuario.Id;
            Session["Username"] = usuario.Username;
            return RedirectToAction("Index", "Encuestas");
        }

        [HttpGet]
        public ActionResult Perfil()
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            var usuario = UsuarioDAL.ObtenerPorUsername(AuthService.GetUsername(Session));
            return View(usuario);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}