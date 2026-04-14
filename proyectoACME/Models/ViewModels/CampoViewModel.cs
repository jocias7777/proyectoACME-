namespace proyectoACME.Models.ViewModels
{
    public class CampoViewModel
    {
        public int Id { get; set; }
        public string NombreCampo { get; set; }
        public string TituloCampo { get; set; }
        public bool EsRequerido { get; set; }
        public string TipoCampo { get; set; }
        public int Orden { get; set; }
    }
}