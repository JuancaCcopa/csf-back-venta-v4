using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using Net.CrossCotting;
using System.Data;

namespace Net.Data
{
    public class RecetaRepository : RepositoryBase<BE_Receta>, IRecetaRepository
    {
        private readonly string _cnx;

        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_RECETAS_POR_FILTRO = DB_ESQUEMA + "VEN_ListaRecetasPorFiltrosGet";
        const string SP_GET_RECETADETALLE_POR_RECETA = DB_ESQUEMA + "VEN_ListaRecetaDetallePorRecetaGet";

        const string SP_GET_RECETA_OBSERVACION_POR_RECETA = DB_ESQUEMA + "VEN_RecetaObservacionGet";

        const string SP_INSERT_RECETA_OBSERVACION = DB_ESQUEMA + "VEN_RecetaObservacionIns";
        const string SP_UPDATE_RECETA_OBSERVACION = DB_ESQUEMA + "VEN_RecetaObservacionUpd";
        const string SP_GET_HOJA_DATOS = DB_ESQUEMA + "VEN_RpHojadeDatosGet";
        public RecetaRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<BE_Receta>> GetListRecetasPorFiltro(DateTime fechainicio, DateTime fechafin, string codtipoconsultamedica, int ide_receta, string nombrespaciente, string sbaestadoreceta)
        {
            ResultadoTransaccion<BE_Receta> vResultadoTransaccion = new ResultadoTransaccion<BE_Receta>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fechainicio = Utilidades.GetFechaHoraInicioActual(fechainicio);
            fechafin = Utilidades.GetFechaHoraFinActual(fechafin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Receta>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RECETAS_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechainicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechafin));
                        cmd.Parameters.Add(new SqlParameter("@codtipoconsultamedica", codtipoconsultamedica == null ? string.Empty : codtipoconsultamedica));
                        cmd.Parameters.Add(new SqlParameter("@ide_receta", ide_receta));
                        cmd.Parameters.Add(new SqlParameter("@nombrespaciente", nombrespaciente == null ? string.Empty : nombrespaciente));
                        cmd.Parameters.Add(new SqlParameter("@sbaestadoreceta", sbaestadoreceta == null ? string.Empty : sbaestadoreceta));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Receta>)context.ConvertTo<BE_Receta>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;
                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_Receta>> GetListRecetasPorReceta(int ide_receta)
        {
            ResultadoTransaccion<BE_Receta> vResultadoTransaccion = new ResultadoTransaccion<BE_Receta>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_Receta>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RECETAS_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ide_receta", ide_receta));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Receta>)context.ConvertTo<BE_Receta>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;
                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_RecetaDetalle>> GetListRecetaDetallePorReceta(int ide_receta)
        {
            ResultadoTransaccion<BE_RecetaDetalle> vResultadoTransaccion = new ResultadoTransaccion<BE_RecetaDetalle>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_RecetaDetalle>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RECETADETALLE_POR_RECETA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ide_receta", ide_receta));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_RecetaDetalle>)context.ConvertTo<BE_RecetaDetalle>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;
                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_RecetaObservacion>> GetListRecetasObservacionPorReceta(int ide_receta)
        {
            ResultadoTransaccion<BE_RecetaObservacion> vResultadoTransaccion = new ResultadoTransaccion<BE_RecetaObservacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_RecetaObservacion>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_RECETA_OBSERVACION_POR_RECETA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id_receta", ide_receta));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_RecetaObservacion>)context.ConvertTo<BE_RecetaObservacion>(reader);
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;
                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;

        }

        public async Task<ResultadoTransaccion<BE_RecetaObservacion>> RegistrarRecetasObservacion(BE_RecetaObservacionXml value)
        {
            ResultadoTransaccion<BE_RecetaObservacion> vResultadoTransaccion = new ResultadoTransaccion<BE_RecetaObservacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT_RECETA_OBSERVACION, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@XmlData", value.XmlData));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);
                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    }
                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_RecetaObservacion>> ModificarRecetasObservacion(BE_RecetaObservacion value)
        {
            ResultadoTransaccion<BE_RecetaObservacion> vResultadoTransaccion = new ResultadoTransaccion<BE_RecetaObservacion>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE_RECETA_OBSERVACION, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@idobs", value.idobs));
                        cmd.Parameters.Add(new SqlParameter("@RegIdUsuario", value.RegIdUsuario));
                        SqlParameter outputIdTransaccionParam = new SqlParameter("@IdTransaccion", SqlDbType.Int, 3)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputIdTransaccionParam);

                        SqlParameter outputMsjTransaccionParam = new SqlParameter("@MsjTransaccion", SqlDbType.VarChar, 700)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMsjTransaccionParam);
                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = int.Parse(outputIdTransaccionParam.Value.ToString());
                        vResultadoTransaccion.ResultadoDescripcion = (string)outputMsjTransaccionParam.Value;
                    }
                }
                catch (Exception ex)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                }
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_HojaDato>> GetRpHojadeDatos(string codatencion)
        {
            ResultadoTransaccion<BE_HojaDato> vResultadoTransaccion = new ResultadoTransaccion<BE_HojaDato>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_HojaDato>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_HOJA_DATOS, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codatencion", codatencion));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var hoja = new BE_HojaDato()
                                {
                                    codatencion = ((reader["codatencion"]) is DBNull) ? string.Empty : reader["codatencion"].ToString().Trim(),
                                    codpaciente = ((reader["codpaciente"]) is DBNull) ? string.Empty : reader["codpaciente"].ToString().Trim(),
                                    nombres = ((reader["nombres"]) is DBNull) ? string.Empty : reader["nombres"].ToString().Trim(),
                                    docidentidad = ((reader["docidentidad"]) is DBNull) ? string.Empty : reader["docidentidad"].ToString().Trim(),
                                    titular = ((reader["titular"]) is DBNull) ? string.Empty : reader["titular"].ToString().Trim(),
                                    fechainicio = ((reader["fechainicio"]) is DBNull) ? string.Empty : reader["fechainicio"].ToString().Trim(),
                                    fechafin = ((reader["fechafin"]) is DBNull) ? string.Empty : reader["fechafin"].ToString().Trim(),
                                    cama = ((reader["cama"]) is DBNull) ? string.Empty : reader["cama"].ToString().Trim(),
                                    login = ((reader["login"]) is DBNull) ? string.Empty : reader["login"].ToString().Trim(),
                                    nombresexo = ((reader["nombresexo"]) is DBNull) ? string.Empty : reader["nombresexo"].ToString().Trim(),
                                    edad = ((reader["edad"]) is DBNull) ? string.Empty : reader["edad"].ToString().Trim(),
                                    nombrecivil = ((reader["nombrecivil"]) is DBNull) ? string.Empty : reader["nombrecivil"].ToString().Trim(),
                                    direccion = ((reader["direccion"]) is DBNull) ? string.Empty : reader["direccion"].ToString().Trim(),
                                    telefono = ((reader["telefono"]) is DBNull) ? string.Empty : reader["telefono"].ToString().Trim(),
                                    nombreprovincia = ((reader["nombreprovincia"]) is DBNull) ? string.Empty : reader["nombreprovincia"].ToString().Trim(),
                                    nombredistrito = ((reader["nombredistrito"]) is DBNull) ? string.Empty : reader["nombredistrito"].ToString().Trim(),
                                    npoliza = ((reader["npoliza"]) is DBNull) ? string.Empty : reader["npoliza"].ToString().Trim(),
                                    nombreaseguradora = ((reader["nombreaseguradora"]) is DBNull) ? string.Empty : reader["nombreaseguradora"].ToString().Trim(),
                                    nombrecontratante = ((reader["nombrecontratante"]) is DBNull) ? string.Empty : reader["nombrecontratante"].ToString().Trim(),
                                    medicotratante = ((reader["medicotratante"]) is DBNull) ? string.Empty : reader["medicotratante"].ToString().Trim(),
                                    observacionesasegurado = ((reader["observacionesasegurado"]) is DBNull) ? string.Empty : reader["observacionesasegurado"].ToString().Trim(),
                                    observacionespaciente = ((reader["observacionespaciente"]) is DBNull) ? string.Empty : reader["observacionespaciente"].ToString().Trim(),
                                    fechainiciovigencia = ((reader["fechainiciovigencia"]) is DBNull) ? string.Empty : reader["fechainiciovigencia"].ToString().Trim(),
                                    fechafinvigencia = ((reader["fechafinvigencia"]) is DBNull) ? string.Empty : reader["fechafinvigencia"].ToString().Trim(),
                                    numerodiasatencion = ((reader["numerodiasatencion"]) is DBNull) ? string.Empty : reader["numerodiasatencion"].ToString().Trim(),
                                    nombretarifa = ((reader["nombretarifa"]) is DBNull) ? string.Empty : reader["nombretarifa"].ToString().Trim(),
                                    nombreparentesco = ((reader["nombreparentesco"]) is DBNull) ? string.Empty : reader["nombreparentesco"].ToString().Trim(),
                                    nombre = ((reader["nombre"]) is DBNull) ? string.Empty : reader["nombre"].ToString().Trim(),
                                    tipomonto = ((reader["tipomonto"]) is DBNull) ? string.Empty : reader["tipomonto"].ToString().Trim(),
                                    monto = ((reader["monto"]) is DBNull) ? 0 : decimal.Parse(reader["monto"].ToString()),
                                    igv = ((reader["igv"]) is DBNull) ? string.Empty : reader["igv"].ToString().Trim(),
                                    observaciones = ((reader["observaciones"]) is DBNull) ? string.Empty : reader["observaciones"].ToString().Trim(),
                                    nombreconcepto = ((reader["nombreconcepto"]) is DBNull) ? string.Empty : reader["nombreconcepto"].ToString().Trim(),
                                    nombremoneda = ((reader["nombremoneda"]) is DBNull) ? string.Empty : reader["nombremoneda"].ToString().Trim(),
                                    paquete = ((reader["paquete"]) is DBNull) ? string.Empty : reader["paquete"].ToString().Trim(),
                                    exclusiones = ((reader["exclusiones"]) is DBNull) ? string.Empty : reader["exclusiones"].ToString().Trim(),
                                    Especialidad = ((reader["Especialidad"]) is DBNull) ? string.Empty : reader["Especialidad"].ToString().Trim(),
                                    est_consulta_medica = ((reader["est_consulta_medica"]) is DBNull) ? string.Empty : reader["est_consulta_medica"].ToString().Trim()
                                };

                                response.Add(hoja);
                            }
                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", response.Count);
                        vResultadoTransaccion.dataList = response;
                    }
                }
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }
    }
}
