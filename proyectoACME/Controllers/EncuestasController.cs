using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using proyectoACME.DAL;
using proyectoACME.Models.Entities;
using proyectoACME.Models.ViewModels;
using proyectoACME.Services;

namespace proyectoACME.Controllers
{
    public class EncuestasController : Controller
    {
        [HttpGet]
        public ActionResult Perfil()
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            int usuarioId = AuthService.GetUsuarioId(Session);
            var usuario = UsuarioDAL.ObtenerPorId(usuarioId);
            return View(usuario);
        }
        [HttpGet]
        public ActionResult Index()
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            int usuarioId = AuthService.GetUsuarioId(Session);
            var encuestas = EncuestaDAL.ObtenerPorUsuario(usuarioId);
            return View(encuestas);
        }

        [HttpGet]
        public ActionResult Crear()
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            return View("_FormEncuesta", new EncuestaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(EncuestaViewModel model)
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            var encuesta = new Encuesta
            {
                UsuarioId = AuthService.GetUsuarioId(Session),
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                TokenLink = TokenService.GenerarToken(),
                Campos = MapearCampos(model.Campos)
            };

            EncuestaDAL.Insertar(encuesta);
            TempData["ToastSuccess"] = "Encuesta creada correctamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            var encuesta = EncuestaDAL.ObtenerPorId(id);
            if (encuesta == null || encuesta.UsuarioId != AuthService.GetUsuarioId(Session))
                return RedirectToAction("Index");

            var model = new EncuestaViewModel
            {
                Id = encuesta.Id,
                Nombre = encuesta.Nombre,
                Descripcion = encuesta.Descripcion,
                Campos = MapearCamposViewModel(encuesta.Campos)
            };
            return View("_FormEncuesta", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(EncuestaViewModel model)
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            var encuesta = new Encuesta
            {
                Id = model.Id,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Campos = MapearCampos(model.Campos)
            };

            EncuestaDAL.Actualizar(encuesta);
            TempData["ToastSuccess"] = "Encuesta actualizada correctamente";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id)
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            EncuestaDAL.Eliminar(id);
            TempData["ToastSuccess"] = "Encuesta eliminada correctamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ToggleEstado(int id)
        {
            try
            {
                if (!AuthService.EstaLogueado(Session))
                    return Json(new { success = false, error = "No autorizado" }, JsonRequestBehavior.AllowGet);

                EncuestaDAL.ToggleEstado(id);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Resultados(int? id)
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            int usuarioId = AuthService.GetUsuarioId(Session);

            // Si no viene id, mostrar resumen general de todas las encuestas
            if (id == null)
            {
                var todasEncuestas = EncuestaDAL.ObtenerPorUsuario(usuarioId);
                ViewBag.Encuesta = null;
                ViewBag.Respuestas = null;
                return View("ResultadosGeneral", todasEncuestas);
            }

            var encuesta = EncuestaDAL.ObtenerPorId(id.Value);
            if (encuesta == null || encuesta.UsuarioId != usuarioId)
                return RedirectToAction("Index");

            var respuestas = RespuestaDAL.ObtenerPorEncuesta(id.Value);
            ViewBag.Encuesta = encuesta;
            ViewBag.Respuestas = respuestas;
            return View("Resultados", encuesta);
        }

        [HttpGet]
        public ActionResult Exportar()
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            int usuarioId = AuthService.GetUsuarioId(Session);
            var encuestas = EncuestaDAL.ObtenerPorUsuario(usuarioId);
            return View(encuestas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportarCSV(int id)
        {
            if (!AuthService.EstaLogueado(Session))
                return RedirectToAction("Login", "Account");

            var encuesta = EncuestaDAL.ObtenerPorId(id);
            var respuestas = RespuestaDAL.ObtenerPorEncuesta(id);

            var sb = new StringBuilder();

            // Cabecera
            sb.Append("#,Fecha,IP");
            foreach (var campo in encuesta.Campos)
                sb.Append("," + "\"" + campo.TituloCampo + "\"");
            sb.AppendLine();

            // Filas
            int num = 1;
            foreach (var resp in respuestas)
            {
                sb.Append(num + "," + resp.FechaRespuesta.ToString("dd/MM/yyyy HH:mm") + "," + resp.IpRespondente);
                foreach (var campo in encuesta.Campos)
                {
                    var detalle = resp.Detalles.Find(d => d.CampoEncuestaId == campo.Id);
                    string valor = detalle != null ? detalle.Valor ?? "" : "";
                    sb.Append(",\"" + valor.Replace("\"", "\"\"") + "\"");
                }
                sb.AppendLine();
                num++;
            }

            byte[] bytes = Encoding.UTF8.GetPreamble();
            byte[] content = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] resultado = new byte[bytes.Length + content.Length];
            bytes.CopyTo(resultado, 0);
            content.CopyTo(resultado, bytes.Length);

            string nombreArchivo = "respuestas_" + encuesta.Nombre.Replace(" ", "_") + ".csv";
            return File(resultado, "text/csv", nombreArchivo);
        }

        // ---------- helpers ----------
        private List<CampoEncuesta> MapearCampos(List<CampoViewModel> vms)
        {
            var lista = new List<CampoEncuesta>();
            if (vms == null) return lista;
            for (int i = 0; i < vms.Count; i++)
            {
                var v = vms[i];
                lista.Add(new CampoEncuesta
                {
                    Id = v.Id,
                    NombreCampo = v.NombreCampo,
                    TituloCampo = v.TituloCampo,
                    EsRequerido = v.EsRequerido,
                    TipoCampo = v.TipoCampo,
                    Orden = i
                });
            }
            return lista;
        }

        private List<CampoViewModel> MapearCamposViewModel(List<CampoEncuesta> entidades)
        {
            var lista = new List<CampoViewModel>();
            if (entidades == null) return lista;
            foreach (var c in entidades)
            {
                lista.Add(new CampoViewModel
                {
                    Id = c.Id,
                    NombreCampo = c.NombreCampo,
                    TituloCampo = c.TituloCampo,
                    EsRequerido = c.EsRequerido,
                    TipoCampo = c.TipoCampo,
                    Orden = c.Orden
                });
            }
            return lista;
        }
    }
}