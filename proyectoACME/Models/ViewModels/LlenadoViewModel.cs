using System.Collections.Generic;
using proyectoACME.Models.Entities;

namespace proyectoACME.Models.ViewModels
{
    public class LlenadoViewModel
    {
        public Encuesta Encuesta { get; set; }
        public List<CampoEncuesta> Campos { get; set; } = new List<CampoEncuesta>();
        public Dictionary<string, string> Valores { get; set; } = new Dictionary<string, string>();
        public string Error { get; set; }
    }
}