using System.Configuration;
using System.Data.SqlClient;

namespace proyectoACME.DAL
{
    public class ConexionDB
    {
        public static SqlConnection ObtenerConexion()
        {
            string cadena = ConfigurationManager.ConnectionStrings["AcmeSurveys"].ConnectionString;
            return new SqlConnection(cadena);
        }
    }
}