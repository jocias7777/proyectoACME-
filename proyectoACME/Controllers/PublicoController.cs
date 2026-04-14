using System.Collections.Generic;
using System.Web.Mvc;
using proyectoACME.DAL;
using proyectoACME.Models.Entities;
using proyectoACME.Models.ViewModels;

namespace proyectoACME.Controllers
{
    public class PublicoController : Controller
    {
        [HttpGet]
        public ActionResult Llenar(string token)
        {
            var encuesta = EncuestaDAL.ObtenerPorToken(token);
            if (encuesta == null || !encuesta.Activa)
                return View("Error404");

            var model = new LlenadoViewModel
            {
                Encuesta = encuesta,
                Campos = encuesta.Campos
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Llenar(string token, FormCollection form)
        {
            var encuesta = EncuestaDAL.ObtenerPorToken(token);
            if (encuesta == null || !encuesta.Activa)
                return View("Error404");

            // Validar campos requeridos
            foreach (var campo in encuesta.Campos)
            {
                if (campo.EsRequerido)
                {
                    string val = form["campo_" + campo.Id];
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        var modelError = new LlenadoViewModel
                        {
                            Encuesta = encuesta,
                            Campos = encuesta.Campos,
                            Error = "Por favor completa todos los campos requeridos."
                        };
                        return View(modelError);
                    }
                }
            }

            var respuesta = new RespuestaEncuesta
            {
                EncuestaId = encuesta.Id,
                IpRespondente = Request.UserHostAddress,
                Detalles = new List<DetalleRespuesta>()
            };

            foreach (var campo in encuesta.Campos)
            {
                respuesta.Detalles.Add(new DetalleRespuesta
                {
                    CampoEncuestaId = campo.Id,
                    Valor = form["campo_" + campo.Id] ?? ""
                });
            }

            RespuestaDAL.Insertar(respuesta);
            return RedirectToAction("Gracias");
        }

        public ActionResult Gracias()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }
    }
}