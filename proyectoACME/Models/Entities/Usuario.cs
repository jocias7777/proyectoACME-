namespace proyectoACME.Models.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public System.DateTime FechaCreacion { get; set; }
    }
}