using System.Collections.Generic;

namespace proyectoACME.Models.ViewModels
{
    public class EncuestaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<CampoViewModel> Campos { get; set; } = new List<CampoViewModel>();
    }
}