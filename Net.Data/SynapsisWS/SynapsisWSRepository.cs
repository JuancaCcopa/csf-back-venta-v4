using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Net.Business.Entities;
using Net.Connection;
using Net.Connection.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Net.Data
{
    public class SynapsisWSRepository : RepositoryBase<BE_SYNAPSIS_MdsynPagos>, ISynapsisWSRepository
    {
        private readonly string _cnx;
        private readonly string _cnxClinica;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");
        private readonly ConnectionServiceLayer _connectServiceLayer;
        //private readonly IRepositoryWrapper _repository;

        const string DB_ESQUEMA = "";
        const string SP_MdsynPagos_Insert = DB_ESQUEMA + "VEN_MdsynPagos_Insert";
        const string SP_MdsynPagos_Update = DB_ESQUEMA + "VEN_MdsynPagos_Update";
        const string SP_MdsynAmReserva_Update = DB_ESQUEMA + "VEN_MdsynAmReserva_Update";
        const string SP_GET_MDSYN_PAGOS_CONSULTA_BY_FILTRO = DB_ESQUEMA + "VEN_MdsynPagos_Consulta";


        //IRepositoryWrapper repository
        public SynapsisWSRepository(IHttpClientFactory clientFactory, IConnectionSQL context, IConfiguration configuration)
           : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _cnxClinica = configuration.GetConnectionString("cnnSqlClinica");
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
            //_repository = repository;

        }

        private Boolean MdsynPagos_Insert(BE_SYNAPSIS_MdsynPagos value, SqlConnection conn, SqlTransaction transaction)
        {
            
            try
            {

                using (SqlCommand cmd = new SqlCommand(SP_MdsynPagos_Insert, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@cod_tipo", value.codTipo));
                    cmd.Parameters.Add(new SqlParameter("@ide_mdsyn_reserva", value.ideMdsynReserva));
                    cmd.Parameters.Add(new SqlParameter("@ide_correl_reserva", value.ideCorrelReserva));
                    cmd.Parameters.Add(new SqlParameter("@cod_liquidacion", value.codLiquidacion));
                    cmd.Parameters.Add(new SqlParameter("@cod_venta", value.codVenta));
                    cmd.Parameters.Add(new SqlParameter("@cnt_monto_pago", value.cntMontoPago));
                    SqlParameter oParam = new SqlParameter("@ide_pagos_bot", SqlDbType.Int, 8)
                    {
                        Value = value.idePagosBot,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(oParam);

                    var resultado = cmd.ExecuteNonQuery();
                    if (resultado >= -1)
                    {
                        value.idePagosBot = Convert.ToInt64(cmd.Parameters["@ide_pagos_bot"].Value);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private Boolean MdsynPagos_Update(BE_SYNAPSIS_MdsynPagos value, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_MdsynPagos_Update, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@cod_tipo", value.codTipo));
                    cmd.Parameters.Add(new SqlParameter("@ide_mdsyn_reserva", value.ideMdsynReserva));
                    cmd.Parameters.Add(new SqlParameter("@ide_correl_reserva", value.ideCorrelReserva));
                    cmd.Parameters.Add(new SqlParameter("@cod_liquidacion", value.codLiquidacion));
                    cmd.Parameters.Add(new SqlParameter("@cod_venta", value.codVenta));

                    cmd.Parameters.Add(new SqlParameter("@cnt_monto_pago", value.cntMontoPago));
                    cmd.Parameters.Add(new SqlParameter("@ide_unique_identifier", value.ideUniqueIdentifier));
                    cmd.Parameters.Add(new SqlParameter("@cod_rpta_synapsis", value.codRptaSynapsis));
                    cmd.Parameters.Add(new SqlParameter("@usr_reg_orden_synapsis", value.usrRegOrdenSynapsis));
                    cmd.Parameters.Add(new SqlParameter("@txt_json_orden", value.txtJsonOrden));

                    cmd.Parameters.Add(new SqlParameter("@est_pagado", value.estPagado));
                    cmd.Parameters.Add(new SqlParameter("@nro_operacion", value.nroOperacion));
                    cmd.Parameters.Add(new SqlParameter("@tip_tarjeta", value.tipTarjeta));
                    cmd.Parameters.Add(new SqlParameter("@num_tarjeta", value.numTarjeta));
                    cmd.Parameters.Add(new SqlParameter("@txt_json_rpta", value.txtJsonRpta));

                    cmd.Parameters.Add(new SqlParameter("@ide_pagos_bot", value.idePagosBot));
                    cmd.Parameters.Add(new SqlParameter("@orden", value.orden));

                    var resultado = cmd.ExecuteNonQuery();
                    if (resultado >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Boolean MdsynAmReserva_Update(BE_MdsynAmReserva value, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(SP_MdsynAmReserva_Update, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ide_correl_reserva", value.ideCorrelReserva));
                    cmd.Parameters.Add(new SqlParameter("@cod_sede", value.codSede));
                    cmd.Parameters.Add(new SqlParameter("@cod_paciente", value.codPaciente));
                    cmd.Parameters.Add(new SqlParameter("@rut_paciente", value.rutPaciente));
                    cmd.Parameters.Add(new SqlParameter("@cod_medico", value.codMedico));

                    cmd.Parameters.Add(new SqlParameter("@cod_prof_medico", value.codProfMedico));
                    cmd.Parameters.Add(new SqlParameter("@cod_especialidad", value.codEspecialidad));
                    cmd.Parameters.Add(new SqlParameter("@fec_cita", value.fecCita));
                    cmd.Parameters.Add(new SqlParameter("@usr_registro_plataforma", value.usrRegistroPlataforma));

                    cmd.Parameters.Add(new SqlParameter("@cnt_montopago", value.cntMontopago));
                    cmd.Parameters.Add(new SqlParameter("@cod_tipo_pago", value.codTipoPago));
                    cmd.Parameters.Add(new SqlParameter("@ide_reserva", value.ideReserva));

                    cmd.Parameters.Add(new SqlParameter("@usr_reserva_anulada", value.usrReservaAnulada));
                    cmd.Parameters.Add(new SqlParameter("@flg_reserva_anulada", value.flgReservaAnulada));
                    cmd.Parameters.Add(new SqlParameter("@orden", value.orden));

                    var resultado = cmd.ExecuteNonQuery();
                    if (resultado >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResultadoTransaccion<B_MdsynPagos> GetMdsynPagosConsulta(long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden, SqlConnection conn, SqlTransaction transaction)
        {
            ResultadoTransaccion<B_MdsynPagos> vResultadoTransaccion = new ResultadoTransaccion<B_MdsynPagos>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {

                using (SqlCommand cmd = new SqlCommand(SP_GET_MDSYN_PAGOS_CONSULTA_BY_FILTRO, conn, transaction))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ide_pagos_bot", ide_pagos_bot));
                    cmd.Parameters.Add(new SqlParameter("@ide_mdsyn_reserva", ide_mdsyn_reserva));
                    cmd.Parameters.Add(new SqlParameter("@ide_correl_reserva", ide_correl_reserva));
                    cmd.Parameters.Add(new SqlParameter("@cod_liquidacion", cod_liquidacion));
                    cmd.Parameters.Add(new SqlParameter("@cod_venta", cod_venta));
                    cmd.Parameters.Add(new SqlParameter("@orden", orden));

                    B_MdsynPagos response = new B_MdsynPagos();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (orden.Equals(2))
                            {
                                response.number = ((reader["number"]) is DBNull) ? 0 : (long)reader["number"];
                                response.amount = ((reader["amount"]) is DBNull) ? 0 : (decimal)reader["amount"];
                                response.cust_name = ((reader["cust_name"]) is DBNull) ? string.Empty : (string)reader["cust_name"];
                                response.cust_lastname = ((reader["cust_lastname"]) is DBNull) ? string.Empty : (string)reader["cust_lastname"];
                                response.cust_phone = ((reader["cust_phone"]) is DBNull) ? string.Empty : (string)reader["cust_phone"];
                                response.cust_email = ((reader["cust_email"]) is DBNull) ? string.Empty : (string)reader["cust_email"];
                                response.cust_doc_type = ((reader["cust_doc_type"]) is DBNull) ? string.Empty : (string)reader["cust_doc_type"];
                                response.cust_doc_number = ((reader["cust_doc_number"]) is DBNull) ? string.Empty : (string)reader["cust_doc_number"];
                                response.cust_adress_country = ((reader["cust_adress_country"]) is DBNull) ? string.Empty : (string)reader["cust_adress_country"];
                                response.cust_adress_levels = ((reader["cust_adress_levels"]) is DBNull) ? string.Empty : (string)reader["cust_adress_levels"];
                                response.cust_adress_line1 = ((reader["cust_adress_line1"]) is DBNull) ? string.Empty : (string)reader["cust_adress_line1"];
                                response.cust_adress_zip = ((reader["cust_adress_zip"]) is DBNull) ? string.Empty : (string)reader["cust_adress_zip"];
                                response.currency_code = ((reader["currency_code"]) is DBNull) ? string.Empty : (string)reader["currency_code"];
                                response.country_code = ((reader["country_code"]) is DBNull) ? string.Empty : (string)reader["country_code"];
                                response.products_name = ((reader["products_name"]) is DBNull) ? string.Empty : (string)reader["products_name"];
                                response.products_quantity = ((reader["products_quantity"]) is DBNull) ? 0 : (int)reader["products_quantity"];
                                response.products_unitAmount = ((reader["products_unitAmount"]) is DBNull) ? 0 : (decimal)reader["products_unitAmount"];
                                response.products_amount = ((reader["products_amount"]) is DBNull) ? 0 : (decimal)reader["products_amount"];
                                response.ordTyp_code = ((reader["ordTyp_code"]) is DBNull) ? string.Empty : (string)reader["ordTyp_code"];
                                response.targTyp_code = ((reader["targTyp_code"]) is DBNull) ? string.Empty : (string)reader["targTyp_code"];
                                response.setting_expiration_date = ((reader["setting_expiration_date"]) is DBNull) ? DateTime.MinValue : (DateTime)reader["setting_expiration_date"];

                            }

                            if (orden.Equals(4))
                            {
                                response.ide_pagos_bot = ((reader["@ide_pagos_bot"]) is DBNull) ? 0 : (int)reader["@ide_pagos_bot"];
                                response.est_pagado = ((reader["@est_pagado"]) is DBNull) ? string.Empty : (string)reader["@est_pagado"];
                                response.nro_operacion = ((reader["@nro_operacion"]) is DBNull) ? string.Empty : (string)reader["@nro_operacion"];
                                response.tip_tarjeta = ((reader["@tip_tarjeta"]) is DBNull) ? string.Empty : (string)reader["@tip_tarjeta"];
                                response.num_tarjeta = ((reader["@num_tarjeta"]) is DBNull) ? string.Empty : (string)reader["@num_tarjeta"];
                                response.cnt_monto_pago = ((reader["@cnt_monto_pago"]) is DBNull) ? 0 : (decimal)reader["@cnt_monto_pago"];
                                response.terminal = ((reader["@terminal"]) is DBNull) ? string.Empty : (string)reader["@terminal"];
                                response.flg_pago_usado = ((reader["@flg_pago_usado"]) is DBNull) ? string.Empty : (string)reader["@flg_pago_usado"];
                            }

                        }
                    }

                    vResultadoTransaccion.IdRegistro = 0;
                    vResultadoTransaccion.ResultadoCodigo = 0;
                    vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", 1);
                    vResultadoTransaccion.data = response;
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

        ////, Synapsis_ApiKey, Synapsis_SignatureKey, Synapsis_Ws_Url
        public ResultadoTransaccion<BE_SYNAPSIS_ResponseOrderApiResult> fnGenerarOrdenPagoBot(BE_SYNAPSIS_MdsynPagos objPagosE, int tipoPago, int UsrSistema,string Synapsis_ApiKey,string Synapsis_SignatureKey,string Synapsis_Ws_Url)
        {
            BE_SYNAPSIS_ResponseOrderApiResult oResult = new BE_SYNAPSIS_ResponseOrderApiResult();
            ResultadoTransaccion<BE_SYNAPSIS_ResponseOrderApiResult> vResultadoTransaccion = new ResultadoTransaccion<BE_SYNAPSIS_ResponseOrderApiResult>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        var respInsert = MdsynPagos_Insert(objPagosE, conn, transaction);

                        if (respInsert)
                        {
                            //VentaCajaRepository vcaja = new VentaCajaRepository(null, context, _configuration);
                            //var obj =  vcaja.GetMdsynPagosConsulta(objPagosE.idePagosBot, 0, 0, "", "", 2);
                            var obj = GetMdsynPagosConsulta(objPagosE.idePagosBot, 0, 0, "", "", 2, conn, transaction);
                            if (obj.data.number > 0)
                            {
                                //var objResultApiKey = _repository.Tabla.GetListTablaClinicaPorFiltros("MEDISYN_SYNAPSIS_APIKEY", "01", 50, 1, -1);
                                //var objApiKey = objResultApiKey.Result.dataList.FirstOrDefault();
                                //string Synapsis_ApiKey = objApiKey.nombre.Trim();

                                oResult = new SynapsisWS().GenerateOrderApiBot(obj.data, Synapsis_ApiKey, Synapsis_SignatureKey, Synapsis_Ws_Url);

                                if (oResult.responseOrderApi.success)
                                {

                                    var number = obj.data.number;
                                    //var number = oResult.responseOrderApi.data.order.number;
                                    var uniqueIdentifier = oResult.responseOrderApi.data.order.uniqueIdentifier;
                                    var set_MdsynPagos = new BE_SYNAPSIS_MdsynPagos(number, "", 0, 0, "", "", 0, uniqueIdentifier, "S", UsrSistema, oResult.jsonBody, "", "", "", "", "", "update_orden_pago");
                                    var resp_MdsynPagos = MdsynPagos_Update(set_MdsynPagos, conn, transaction);
                                    var set_MdsynAmReserva = new BE_MdsynAmReserva(objPagosE.ideMdsynReserva, 0, "", "", "", "", "", "", DateTime.Now, "", objPagosE.cntMontoPago, "B", "update_tipo_pago", 0, "");
                                    var resp_MdsynAmReserva = MdsynAmReserva_Update(set_MdsynAmReserva, conn, transaction);

                                    transaction.Commit();
                                    vResultadoTransaccion.ResultadoCodigo = 0;
                                    vResultadoTransaccion.data = oResult;
                                    vResultadoTransaccion.ResultadoDescripcion = "Se generó la orden del pago. Nro: " + number.ToString();

                                }
                                else {
                                    throw new ArgumentException(oResult.responseOrderApi.message.code + " " + oResult.responseOrderApi.message.text);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = ex.Message;
                        transaction.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message;
            }

            return vResultadoTransaccion;

        }

    }

}
