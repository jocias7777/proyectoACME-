namespace proyectoACME.Models.Entities
{
    public class DetalleRespuesta
    {
        public int Id { get; set; }
        public int RespuestaEncuestaId { get; set; }
        public int CampoEncuestaId { get; set; }
        public string Valor { get; set; }
    }
}