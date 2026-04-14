using System;
using System.Collections.Generic;

namespace proyectoACME.Models.Entities
{
    public class RespuestaEncuesta
    {
        public int Id { get; set; }
        public int EncuestaId { get; set; }
        public DateTime FechaRespuesta { get; set; }
        public string IpRespondente { get; set; }
        public List<DetalleRespuesta> Detalles { get; set; } = new List<DetalleRespuesta>();
    }
}