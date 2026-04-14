using System;
using System.Collections.Generic;

namespace proyectoACME.Models.Entities
{
    public class Encuesta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string TokenLink { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activa { get; set; }
        public List<CampoEncuesta> Campos { get; set; } = new List<CampoEncuesta>();
        public int TotalRespuestas { get; set; }
    }
}