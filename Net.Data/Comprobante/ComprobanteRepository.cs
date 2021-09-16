using Net.Business.Entities;
using Net.Connection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;
using System.Text.RegularExpressions;
using Net.CrossCotting;

namespace Net.Data
{
    public class ComprobanteRepository : RepositoryBase<BE_Comprobante>, IComprobanteRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_LISTA_COMPROBANTES_POR_FILTRO = DB_ESQUEMA + "VEN_ListaComprobantesPorFiltrosGet";
        const string SP_GET_COMPROBANTE_ELECTRONICO = DB_ESQUEMA + "VEN_ComprobantesElectronicosGet";
        const string SP_GET_CUADRECAJA_PANTALLA = DB_ESQUEMA + "VEN_Cuadredecaja_Pantalla";
        const string COMPROBANTE_DELETE = DB_ESQUEMA + "VEN_Comprobantes_Delete";
        const string COMPROBANTE_UPDATE = DB_ESQUEMA + "VEN_Comprobantes_Update";
        const string SP_GET_COMPROBANTE = DB_ESQUEMA + "VEN_Comprobantes_Consulta";

        public ComprobanteRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
        }

        public async Task<ResultadoTransaccion<BE_Comprobante>> GetListaComprobantesPorFiltro(string codcomprobante, DateTime fecinicio, DateTime fecfin, int opcion)
        {
            ResultadoTransaccion<BE_Comprobante> vResultadoTransaccion = new ResultadoTransaccion<BE_Comprobante>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            fecinicio = Utilidades.GetFechaHoraInicioActual(fecinicio);
            fecfin = Utilidades.GetFechaHoraFinActual(fecfin);

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                   
                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_COMPROBANTES_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@fecinicio", fecinicio));
                        cmd.Parameters.Add(new SqlParameter("@fecfin", fecfin));
                        cmd.Parameters.Add(new SqlParameter("@opcion", opcion));

                        conn.Open();

                        var response = new List<BE_Comprobante>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Comprobante>)context.ConvertTo<BE_Comprobante>(reader);
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
        public async Task<ResultadoTransaccion<BE_Comprobante>> GetComprobantesElectronico(string codcomprobante_e, string codsistema)
        {
            ResultadoTransaccion<BE_Comprobante> vResultadoTransaccion = new ResultadoTransaccion<BE_Comprobante>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_COMPROBANTE_ELECTRONICO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante_e", codcomprobante_e));
                        cmd.Parameters.Add(new SqlParameter("@codsistema", codsistema));

                        conn.Open();

                        var response = new BE_Comprobante();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.codventa = ((reader["codventa"]) is DBNull) ? string.Empty : reader["codventa"].ToString().Trim();
                                response.codcomprobante = ((reader["codcomprobante"]) is DBNull) ? string.Empty : reader["codcomprobante"].ToString().Trim();
                                response.flg_gratuito = ((reader["flg_gratuito"]) is DBNull) ? false : Convert.ToBoolean(reader["flg_gratuito"]);
                                response.flg_electronico = ((reader["flg_electronico"]) is DBNull) ? false : Convert.ToBoolean(reader["flg_electronico"]);
                                response.codcliente = ((reader["codcliente"]) is DBNull) ? string.Empty : reader["codcliente"].ToString().Trim();
                                response.anombrede = ((reader["anombrede"]) is DBNull) ? string.Empty : reader["anombrede"].ToString().Trim();
                                response.direccion = ((reader["direccion"]) is DBNull) ? string.Empty : reader["direccion"].ToString().Trim();
                                response.ruc = ((reader["ruc"]) is DBNull) ? string.Empty : reader["ruc"].ToString().Trim();
                                response.correo = ((reader["correo"]) is DBNull) ? string.Empty : reader["correo"].ToString().Trim();
                                response.tipdocidentidad = ((reader["tipdocidentidad"]) is DBNull) ? string.Empty : reader["tipdocidentidad"].ToString().Trim();
                                response.nombretipdocidentidad = ((reader["nombretipdocidentidad"]) is DBNull) ? string.Empty : reader["nombretipdocidentidad"].ToString().Trim();
                                response.docidentidad = ((reader["docidentidad"]) is DBNull) ? string.Empty : reader["docidentidad"].ToString().Trim();
                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : Convert.ToDateTime(reader["fechaemision"]);
                                response.nombretipocliente = ((reader["nombretipocliente"]) is DBNull) ? string.Empty : reader["nombretipocliente"].ToString().Trim();
                                response.codtipocliente = ((reader["codtipocliente"]) is DBNull) ? string.Empty : reader["codtipocliente"].ToString().Trim();
                                response.nombretipocliente = ((reader["nombretipocliente"]) is DBNull) ? string.Empty : reader["nombretipocliente"].ToString().Trim();
                                response.codplan = ((reader["codplan"]) is DBNull) ? string.Empty : reader["codplan"].ToString().Trim();
                                response.nombreplan = ((reader["nombreplan"]) is DBNull) ? string.Empty : reader["nombreplan"].ToString().Trim();
                                response.porcentajedctoplan = ((reader["porcentajedctoplan"]) is DBNull) ? 0 : (decimal)reader["porcentajedctoplan"];
                                response.porcentajecoaseguro = ((reader["porcentajecoaseguro"]) is DBNull) ? 0 : (decimal)reader["porcentajecoaseguro"];
                                response.montototal = ((reader["montototal"]) is DBNull) ? 0 : (decimal)reader["montototal"];
                                response.montoigv = ((reader["montoigv"]) is DBNull) ? 0 : (decimal)reader["montoigv"];
                                response.cardcode = ((reader["cardcode"]) is DBNull) ? string.Empty : reader["cardcode"].ToString().Trim();
                                response.porcentajeimpuesto = ((reader["porcentajeimpuesto"]) is DBNull) ? 0 : (decimal)reader["porcentajeimpuesto"];
                                response.estado = ((reader["estado"]) is DBNull) ? string.Empty : reader["estado"].ToString().Trim();
                                response.moneda = ((reader["moneda"]) is DBNull) ? string.Empty : reader["moneda"].ToString().Trim();
                                response.nombreestado = ((reader["nombreestado"]) is DBNull) ? string.Empty : reader["nombreestado"].ToString().Trim();
                                response.tipoafectacionigv = ((reader["tipoafectacionigv"]) is DBNull) ? string.Empty : reader["tipoafectacionigv"].ToString().Trim();

                            }

                            BE_CuadreCaja cuadreCaja;
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    cuadreCaja = new BE_CuadreCaja();
                                    cuadreCaja.tipopago = ((reader["tipopago"]) is DBNull) ? string.Empty : reader["tipopago"].ToString().Trim();
                                    cuadreCaja.nombretipopago = ((reader["nombretipopago"]) is DBNull) ? string.Empty : reader["nombretipopago"].ToString().Trim();
                                    cuadreCaja.nombreentidad = ((reader["nombreentidad"]) is DBNull) ? string.Empty : reader["nombreentidad"].ToString().Trim();
                                    //cuadreCaja.descripcionentidad = ((reader["descripcionentidad"]) is DBNull) ? string.Empty : reader["descripcionentidad"].ToString().Trim();
                                    cuadreCaja.descripcionentidad = ((reader["nombrenombreentidad"]) is DBNull) ? string.Empty : reader["nombrenombreentidad"].ToString().Trim();
                                    cuadreCaja.numeroentidad = ((reader["numeroentidad"]) is DBNull) ? string.Empty : reader["numeroentidad"].ToString().Trim();
                                    cuadreCaja.monto = ((reader["monto"]) is DBNull) ? 0 : Convert.ToDecimal(reader["monto"]);
                                    cuadreCaja.montodolares = ((reader["montodolares"]) is DBNull) ? 0 : Convert.ToDecimal(reader["montodolares"]);
                                    cuadreCaja.moneda = ((reader["moneda"]) is DBNull) ? string.Empty : reader["moneda"].ToString().Trim();
                                    cuadreCaja.codterminal = ((reader["codterminal"]) is DBNull) ? string.Empty : reader["codterminal"].ToString().Trim();
                                    cuadreCaja.numeroterminal = ((reader["numeroterminal"]) is DBNull) ? string.Empty : reader["numeroterminal"].ToString().Trim();
                                    response.cuadreCaja.Add(cuadreCaja);
                                }
                            }

                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = response;

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
        public async Task<ResultadoTransaccion<BE_CuadreCaja>> GetListaCuadreCaja(string documento)
        {
            ResultadoTransaccion<BE_CuadreCaja> vResultadoTransaccion = new ResultadoTransaccion<BE_CuadreCaja>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_CUADRECAJA_PANTALLA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@documento", documento));

                        conn.Open();

                        BE_CuadreCaja response;
                        IList<BE_CuadreCaja> lista = new List<BE_CuadreCaja>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = new BE_CuadreCaja();
                                response.tipopago = ((reader["tipopago"]) is DBNull) ? string.Empty : reader["tipopago"].ToString().Trim();
                                response.nombretipopago = ((reader["nombretipopago"]) is DBNull) ? string.Empty : reader["nombretipopago"].ToString().Trim();
                                response.nombreentidad = ((reader["nombreentidad"]) is DBNull) ? string.Empty : reader["nombreentidad"].ToString().Trim();
                                response.descripcionentidad = ((reader["descripcionentidad"]) is DBNull) ? string.Empty : reader["descripcionentidad"].ToString().Trim();
                                response.numeroentidad = ((reader["numeroentidad"]) is DBNull) ? string.Empty : reader["numeroentidad"].ToString().Trim();
                                response.monto = ((reader["monto"]) is DBNull) ? 0 : Convert.ToDecimal(reader["monto"]);
                                response.montodolares = ((reader["montodolares"]) is DBNull) ? 0 : Convert.ToDecimal(reader["montodolares"]);
                                response.moneda = ((reader["moneda"]) is DBNull) ? string.Empty : reader["moneda"].ToString().Trim();
                                response.codterminal = ((reader["codterminal"]) is DBNull) ? string.Empty : reader["codterminal"].ToString().Trim();
                                response.numeroterminal = ((reader["numeroterminal"]) is DBNull) ? string.Empty : reader["numeroterminal"].ToString().Trim();
                                response.fechaemision = ((reader["fechaemision"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechaemision"];
                                response.fechacancelacion = ((reader["fechacancelacion"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["fechacancelacion"];
                                response.numeroplanilla = ((reader["numeroplanilla"]) is DBNull) ? string.Empty : (string)reader["numeroplanilla"];
                                response.codventa = ((reader["codventa"]) is DBNull) ? string.Empty : (string)reader["codventa"];
                                response.estado = ((reader["estado"]) is DBNull) ? string.Empty : (string)reader["estado"];
                                response.documento = ((reader["documento"]) is DBNull) ? string.Empty : (string)reader["documento"];
                                lista.Add(response);

                            }

                        }

                        conn.Close();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", lista.Count);
                        vResultadoTransaccion.dataList = lista;

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
        public async Task<ResultadoTransaccion<string>> ComprobanteDelete(string codcomprobante)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(COMPROBANTE_DELETE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));

                        await conn.OpenAsync();
                        var reader = await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                        vResultadoTransaccion.data = codcomprobante;

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

        public async Task<ResultadoTransaccion<BE_Comprobante>> GetComprobanteConsulta(string buscar, int key, int numerolineas, int orden)
        {
            ResultadoTransaccion<BE_Comprobante> vResultadoTransaccion = new ResultadoTransaccion<BE_Comprobante>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET_COMPROBANTE, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@buscar", buscar));
                        cmd.Parameters.Add(new SqlParameter("@key", key));
                        cmd.Parameters.Add(new SqlParameter("@numerolineas", numerolineas));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        List<BE_Comprobante> ListaResponse = new List<BE_Comprobante>();
                        var response = new BE_Comprobante();

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = new BE_Comprobante();
                                response.numeroplanilla = ((reader["NumeroPlanilla"]) is DBNull) ? string.Empty : reader["NumeroPlanilla"].ToString().Trim();
                                response.fechacancelacion = ((reader["fechacancelacion"]) is DBNull) ? (DateTime?)null : (DateTime)reader["fechacancelacion"];
                                response.estado = ((reader["Estado"]) is DBNull) ? string.Empty : reader["Estado"].ToString().Trim();
                                ListaResponse.Add(response);
                            }

                        }

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 2);
                        vResultadoTransaccion.dataList = ListaResponse;

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

        public async Task<ResultadoTransaccion<string>> ComprobantesUpdate(string campo, string codigo, string nuevovalor)
        {
            ResultadoTransaccion<string> vResultadoTransaccion = new ResultadoTransaccion<string>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(COMPROBANTE_UPDATE, conn, transaction))
                        {

                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@campo", campo));
                            cmd.Parameters.Add(new SqlParameter("@codigo", codigo));
                            cmd.Parameters.Add(new SqlParameter("@nuevovalor", nuevovalor));

                            await cmd.ExecuteNonQueryAsync();

                            vResultadoTransaccion.IdRegistro = 0;
                            vResultadoTransaccion.ResultadoCodigo = 0;
                            vResultadoTransaccion.ResultadoDescripcion = "SE ACTUALIZO CORRECTAMENTE";
                            vResultadoTransaccion.data = codigo;

                        }

                        transaction.Commit();
                        transaction.Dispose();

                    }
                    catch (Exception ex)
                    {
                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex3)
                        {
                            vResultadoTransaccion.ResultadoDescripcion = ex3.Message.ToString();
                        }
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
