using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using proyectoACME.Models.Entities;

namespace proyectoACME.DAL
{
    public class EncuestaDAL
    {
        public static List<Encuesta> ObtenerPorUsuario(int usuarioId)
        {
            var lista = new List<Encuesta>();
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                string sql = @"
                    SELECT e.Id, e.Nombre, e.Descripcion, e.TokenLink, e.FechaCreacion, e.Activa,
                           (SELECT COUNT(*) FROM dbo.CamposEncuesta WHERE EncuestaId = e.Id) AS TotalCampos,
                           (SELECT COUNT(*) FROM dbo.RespuestasEncuesta WHERE EncuestaId = e.Id) AS TotalRespuestas
                    FROM dbo.Encuestas e
                    WHERE e.UsuarioId = @UsuarioId
                    ORDER BY e.FechaCreacion DESC";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = usuarioId });
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var enc = new Encuesta
                            {
                                Id = (int)dr["Id"],
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"] == DBNull.Value ? "" : dr["Descripcion"].ToString(),
                                TokenLink = dr["TokenLink"].ToString(),
                                FechaCreacion = (DateTime)dr["FechaCreacion"],
                                Activa = (bool)dr["Activa"],
                                TotalRespuestas = (int)dr["TotalRespuestas"]
                            };
                            enc.Campos = new List<CampoEncuesta>();
                            for (int i = 0; i < (int)dr["TotalCampos"]; i++)
                                enc.Campos.Add(new CampoEncuesta());
                            lista.Add(enc);
                        }
                    }
                }
            }
            return lista;
        }

        public static Encuesta ObtenerPorId(int id)
        {
            Encuesta enc = null;
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT Id, UsuarioId, Nombre, Descripcion, TokenLink, FechaCreacion, Activa FROM dbo.Encuestas WHERE Id = @Id", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            enc = new Encuesta
                            {
                                Id = (int)dr["Id"],
                                UsuarioId = (int)dr["UsuarioId"],
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"] == DBNull.Value ? "" : dr["Descripcion"].ToString(),
                                TokenLink = dr["TokenLink"].ToString(),
                                FechaCreacion = (DateTime)dr["FechaCreacion"],
                                Activa = (bool)dr["Activa"]
                            };
                        }
                    }
                }
                if (enc != null)
                    enc.Campos = ObtenerCampos(enc.Id, con);
            }
            return enc;
        }

        public static Encuesta ObtenerPorToken(string token)
        {
            Encuesta enc = null;
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT Id, UsuarioId, Nombre, Descripcion, TokenLink, FechaCreacion, Activa FROM dbo.Encuestas WHERE TokenLink = @Token", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Token", SqlDbType.NVarChar, 64) { Value = token });
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            enc = new Encuesta
                            {
                                Id = (int)dr["Id"],
                                UsuarioId = (int)dr["UsuarioId"],
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"] == DBNull.Value ? "" : dr["Descripcion"].ToString(),
                                TokenLink = dr["TokenLink"].ToString(),
                                FechaCreacion = (DateTime)dr["FechaCreacion"],
                                Activa = (bool)dr["Activa"]
                            };
                        }
                    }
                }
                if (enc != null)
                    enc.Campos = ObtenerCampos(enc.Id, con);
            }
            return enc;
        }

        public static List<CampoEncuesta> ObtenerCampos(int encuestaId, SqlConnection con = null)
        {
            var lista = new List<CampoEncuesta>();
            bool cerrar = con == null;
            if (con == null) { con = ConexionDB.ObtenerConexion(); con.Open(); }
            using (var cmd = new SqlCommand(
                "SELECT Id, EncuestaId, NombreCampo, TituloCampo, EsRequerido, TipoCampo, Orden FROM dbo.CamposEncuesta WHERE EncuestaId = @EncuestaId ORDER BY Orden", con))
            {
                cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = encuestaId });
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new CampoEncuesta
                        {
                            Id = (int)dr["Id"],
                            EncuestaId = (int)dr["EncuestaId"],
                            NombreCampo = dr["NombreCampo"].ToString(),
                            TituloCampo = dr["TituloCampo"].ToString(),
                            EsRequerido = (bool)dr["EsRequerido"],
                            TipoCampo = dr["TipoCampo"].ToString(),
                            Orden = (int)dr["Orden"]
                        });
                    }
                }
            }
            if (cerrar) con.Dispose();
            return lista;
        }

        public static int Insertar(Encuesta enc)
        {
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        int newId;
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO dbo.Encuestas (UsuarioId, Nombre, Descripcion, TokenLink, Activa)
                              VALUES (@UsuarioId, @Nombre, @Descripcion, @TokenLink, 1);
                              SELECT SCOPE_IDENTITY();", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = enc.UsuarioId });
                            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 200) { Value = enc.Nombre });
                            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 500) { Value = (object)enc.Descripcion ?? DBNull.Value });
                            cmd.Parameters.Add(new SqlParameter("@TokenLink", SqlDbType.NVarChar, 64) { Value = enc.TokenLink });
                            newId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        InsertarCampos(newId, enc.Campos, con, tran);
                        tran.Commit();
                        return newId;
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public static bool TieneRespuestas(int encuestaId)
        {
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.RespuestasEncuesta WHERE EncuestaId = @EncuestaId", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = encuestaId });
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public static void Actualizar(Encuesta enc)
        {
            if (TieneRespuestas(enc.Id))
            {
                throw new InvalidOperationException("No se puede editar una encuesta que ya tiene respuestas.");
            }
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(
                            "UPDATE dbo.Encuestas SET Nombre=@Nombre, Descripcion=@Descripcion WHERE Id=@Id", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 200) { Value = enc.Nombre });
                            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 500) { Value = (object)enc.Descripcion ?? DBNull.Value });
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = enc.Id });
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SqlCommand(
                            "DELETE FROM dbo.CamposEncuesta WHERE EncuestaId = @EncuestaId", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = enc.Id });
                            cmd.ExecuteNonQuery();
                        }
                        InsertarCampos(enc.Id, enc.Campos, con, tran);
                        tran.Commit();
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public static void ToggleEstado(int id)
        {
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    "UPDATE dbo.Encuestas SET Activa = CASE WHEN Activa = 1 THEN 0 ELSE 1 END WHERE Id = @Id", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Eliminar(int id)
        {
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        // Obtener todos los IDs de respuestas de esta encuesta
                        var respuestasIds = new List<int>();
                        using (var cmd = new SqlCommand("SELECT Id FROM dbo.RespuestasEncuesta WHERE EncuestaId = @EncuestaId", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = id });
                            using (var dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    respuestasIds.Add((int)dr["Id"]);
                                }
                            }
                        }

                        // Eliminar los detalles de cada respuesta
                        foreach (var respuestaId in respuestasIds)
                        {
                            using (var cmd = new SqlCommand("DELETE FROM dbo.DetalleRespuesta WHERE RespuestaEncuestaId = @RespuestaId", con, tran))
                            {
                                cmd.Parameters.Add(new SqlParameter("@RespuestaId", SqlDbType.Int) { Value = respuestaId });
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Eliminar las respuestas de la encuesta
                        using (var cmd = new SqlCommand("DELETE FROM dbo.RespuestasEncuesta WHERE EncuestaId = @EncuestaId", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = id });
                            cmd.ExecuteNonQuery();
                        }

                        // Eliminar los campos de la encuesta
                        using (var cmd = new SqlCommand("DELETE FROM dbo.CamposEncuesta WHERE EncuestaId = @EncuestaId", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = id });
                            cmd.ExecuteNonQuery();
                        }

                        // Finalmente eliminar la encuesta
                        using (var cmd = new SqlCommand("DELETE FROM dbo.Encuestas WHERE Id = @Id", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void InsertarCampos(int encuestaId, List<CampoEncuesta> campos, SqlConnection con, SqlTransaction tran)
        {
            for (int i = 0; i < campos.Count; i++)
            {
                var c = campos[i];
                using (var cmd = new SqlCommand(
                    @"INSERT INTO dbo.CamposEncuesta (EncuestaId, NombreCampo, TituloCampo, EsRequerido, TipoCampo, Orden)
                      VALUES (@EncuestaId, @NombreCampo, @TituloCampo, @EsRequerido, @TipoCampo, @Orden)", con, tran))
                {
                    cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = encuestaId });
                    cmd.Parameters.Add(new SqlParameter("@NombreCampo", SqlDbType.NVarChar, 100) { Value = c.NombreCampo });
                    cmd.Parameters.Add(new SqlParameter("@TituloCampo", SqlDbType.NVarChar, 200) { Value = c.TituloCampo });
                    cmd.Parameters.Add(new SqlParameter("@EsRequerido", SqlDbType.Bit) { Value = c.EsRequerido });
                    cmd.Parameters.Add(new SqlParameter("@TipoCampo", SqlDbType.NVarChar, 10) { Value = c.TipoCampo });
                    cmd.Parameters.Add(new SqlParameter("@Orden", SqlDbType.Int) { Value = i });
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}