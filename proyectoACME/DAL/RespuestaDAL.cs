using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using proyectoACME.Models.Entities;

namespace proyectoACME.DAL
{
    public class RespuestaDAL
    {
        public static void Insertar(RespuestaEncuesta respuesta)
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
                            @"INSERT INTO dbo.RespuestasEncuesta (EncuestaId, IpRespondente)
                              VALUES (@EncuestaId, @Ip);
                              SELECT SCOPE_IDENTITY();", con, tran))
                        {
                            cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = respuesta.EncuestaId });
                            cmd.Parameters.Add(new SqlParameter("@Ip", SqlDbType.NVarChar, 50) { Value = (object)respuesta.IpRespondente ?? DBNull.Value });
                            newId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        foreach (var detalle in respuesta.Detalles)
                        {
                            using (var cmd = new SqlCommand(
                                @"INSERT INTO dbo.DetalleRespuesta (RespuestaEncuestaId, CampoEncuestaId, Valor)
                                  VALUES (@RespuestaId, @CampoId, @Valor)", con, tran))
                            {
                                cmd.Parameters.Add(new SqlParameter("@RespuestaId", SqlDbType.Int) { Value = newId });
                                cmd.Parameters.Add(new SqlParameter("@CampoId", SqlDbType.Int) { Value = detalle.CampoEncuestaId });
                                cmd.Parameters.Add(new SqlParameter("@Valor", SqlDbType.NVarChar, -1) { Value = (object)detalle.Valor ?? DBNull.Value });
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public static List<RespuestaEncuesta> ObtenerPorEncuesta(int encuestaId)
        {
            var lista = new List<RespuestaEncuesta>();
            using (var con = ConexionDB.ObtenerConexion())
            {
                con.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT r.Id, r.EncuestaId, r.FechaRespuesta, r.IpRespondente,
                             d.Id AS DetalleId, d.CampoEncuestaId, d.Valor
                      FROM dbo.RespuestasEncuesta r
                      LEFT JOIN dbo.DetalleRespuesta d ON d.RespuestaEncuestaId = r.Id
                      WHERE r.EncuestaId = @EncuestaId
                      ORDER BY r.Id, d.CampoEncuestaId", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@EncuestaId", SqlDbType.Int) { Value = encuestaId });
                    using (var dr = cmd.ExecuteReader())
                    {
                        RespuestaEncuesta actual = null;
                        while (dr.Read())
                        {
                            int rid = (int)dr["Id"];
                            if (actual == null || actual.Id != rid)
                            {
                                actual = new RespuestaEncuesta
                                {
                                    Id = rid,
                                    EncuestaId = (int)dr["EncuestaId"],
                                    FechaRespuesta = (DateTime)dr["FechaRespuesta"],
                                    IpRespondente = dr["IpRespondente"] == DBNull.Value ? "" : dr["IpRespondente"].ToString(),
                                    Detalles = new List<DetalleRespuesta>()
                                };
                                lista.Add(actual);
                            }
                            if (dr["DetalleId"] != DBNull.Value)
                            {
                                actual.Detalles.Add(new DetalleRespuesta
                                {
                                    Id = (int)dr["DetalleId"],
                                    RespuestaEncuestaId = rid,
                                    CampoEncuestaId = (int)dr["CampoEncuestaId"],
                                    Valor = dr["Valor"] == DBNull.Value ? "" : dr["Valor"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            return lista;
        }
    }
}