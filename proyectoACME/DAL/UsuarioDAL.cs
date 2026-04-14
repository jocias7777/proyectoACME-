using System.Data;
using System.Data.SqlClient;
using proyectoACME.Models.Entities;

namespace proyectoACME.DAL
{
    public class UsuarioDAL
    {
        public static Usuario ObtenerPorId(int id)
        {
            Usuario usuario = null;
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT Id, Username, PasswordHash, FechaCreacion FROM dbo.Usuarios WHERE Id = @Id", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = (int)dr["Id"],
                                Username = dr["Username"].ToString(),
                                PasswordHash = dr["PasswordHash"].ToString(),
                                FechaCreacion = (System.DateTime)dr["FechaCreacion"]
                            };
                        }
                    }
                }
            }
            return usuario;
        }
        public static Usuario ObtenerPorUsername(string username)
        {
            Usuario usuario = null;
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT Id, Username, PasswordHash, FechaCreacion FROM dbo.Usuarios WHERE Username = @Username", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 100) { Value = username });
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = (int)dr["Id"],
                                Username = dr["Username"].ToString(),
                                PasswordHash = dr["PasswordHash"].ToString(),
                                FechaCreacion = (System.DateTime)dr["FechaCreacion"]
                            };
                        }
                    }
                }
            }
            return usuario;
        }
    }
}