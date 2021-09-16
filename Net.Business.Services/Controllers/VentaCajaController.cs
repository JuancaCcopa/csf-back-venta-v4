using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Business.Entities;
using Net.Data;
using wsComprobanteTCI;
using static wsComprobanteTCI.WSComprobanteSoapClient;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiVenta")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class VentaCajaController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;

        public VentaCajaController(IRepositoryWrapper repository)
        {
            this._repository = repository;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codventa"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVentaCabeceraPorCodVenta([FromQuery] string codventa)
        {

            var objectGetAll = await _repository.VentaCaja.GetVentaCabeceraPorCodVenta(codventa);
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            if (objectGetAll.data.codventa == null) return BadRequest($"El codigo de la venta no existe: {codventa}");

            decimal montoNeto = 0;
            if (objectGetAll.data.codtipocliente.Equals("01"))
            {
                montoNeto = objectGetAll.data.montopaciente;
            }
            if (objectGetAll.data.codtipocliente.Equals("02") || objectGetAll.data.codtipocliente.Equals("03") || objectGetAll.data.codtipocliente.Equals("04"))
            {
                montoNeto = objectGetAll.data.montoaseguradora;
            }

            objectGetAll.data.montoigv = decimal.Round((montoNeto * (objectGetAll.data.porcentajeimpuesto / 100)), 2);
            objectGetAll.data.montoneto = decimal.Round(montoNeto, 2);
            objectGetAll.data.montototal = decimal.Round((montoNeto + objectGetAll.data.montoigv), 2);
            if (objectGetAll.data.flg_gratuito)
            {
                objectGetAll.data.montototal = 0;
            }


            return Ok(objectGetAll.data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <param name="codCliente"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDatoCardCodeConsulta([FromQuery] string tipoCliente, string codCliente)
        {
            var objectGetAll = await _repository.VentaCaja.GetDatoCardCodeConsulta(tipoCliente, codCliente);
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll);
        }

        //GetDatoCardCodeConsulta

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codventa"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVentaDetallePorCodVenta([FromQuery] string codventa)
        {
            var objectGetAll = await _repository.VentaCaja.GetVentaDetallePorCodVenta(codventa);
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }
            return Ok(objectGetAll.dataList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codPaciente"></param>
        /// <param name="codAseguradora"></param>
        /// <param name="codCia"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRucConsultav2PorFiltro([FromQuery] string codPaciente, string codAseguradora, string codCia)
        {
            var objectGetAll = await _repository.VentaCaja.GetRucConsultav2PorFiltro(codPaciente.Trim(), codAseguradora.Trim(), codCia.Trim());
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }
            return Ok(objectGetAll.data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ide_pagos_bot"></param>
        /// <param name="ide_mdsyn_reserva"></param>
        /// <param name="ide_correl_reserva"></param>
        /// <param name="cod_liquidacion"></param>
        /// <param name="cod_venta"></param>
        /// <param name="orden"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMdsynPagosConsulta([FromQuery] long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden)
        {
            cod_liquidacion = cod_liquidacion is null ? string.Empty : cod_liquidacion;
            var objectGetAll = await _repository.VentaCaja.GetMdsynPagosConsulta(ide_pagos_bot, ide_mdsyn_reserva, ide_correl_reserva, cod_liquidacion, cod_venta, orden);
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            if (orden == 6)
            {

                var res = new { exito = (objectGetAll.data.link == null) ? 0 : 1, mensaje = (objectGetAll.data.link == null) ? "Esta venta no tiene ninguna orden de pago" : objectGetAll.data.link };
                return Ok(res);

            }

            return Ok(objectGetAll.data);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codPersonal"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLimiteConsumoPersonalPorCodPersonal([FromQuery] string codPersonal)
        {
            var objectGetAll = await _repository.VentaCaja.GetLimiteConsumoPersonalPorCodPersonal(codPersonal);
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerarPagar([FromBody] DtoComprobanteCabeceraRegistrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                string wStrURL = string.Empty;

                if (value.wFlg_electronico)
                {
                    var objWebServices = await _repository.VentaCaja.GetWebServicesPorCodTabla("EFACT_TCI_WS");
                    if (objWebServices.IdRegistro.Equals(0))
                    {
                        wStrURL = objWebServices.data;
                    }
                    else
                    {
                        return Ok("No se tiene configurado en url del servicio");
                    }
                }

                //code 18 -> Verifica TipoMovimiento de Venta y code 27 Verifica Datos de Venta
                string strCodCliente = string.Empty;
                var objVentasCabecera = await _repository.VentaCaja.GetVentaCabeceraPorCodVenta(value.codVenta);
                if (objVentasCabecera.IdRegistro == 0)
                {
                    if (objVentasCabecera.data.codcomprobante.Trim() != "")
                    {
                        var resul = new { exito = false, mensaje = $"NO puede Facturar, Venta tiene Comprobante : {objVentasCabecera.data.codcomprobante.Trim()}" };
                        return Ok(resul);
                    }
                    if (objVentasCabecera.data.tipomovimiento.Equals("DV") && objVentasCabecera.data.codcomprobante.Trim() == "")
                    {
                        //Beep graba
                    }
                    else
                    {
                        var resul = new { exito = false, mensaje = $"Verifique el TipoMovimiento de la Venta" };
                        return Ok(resul);
                    }

                    if (objVentasCabecera.data.strTienedevolucion.Equals("S"))
                    {
                        var resul = new { exito = false, mensaje = "No puede facturar, la venta tiene una devolución asociada o esta anulada" };
                        return Ok(resul);
                    }

                    strCodCliente = objVentasCabecera.data.codcliente;
                }
                else
                {
                    var resul = new { exito = false, mensaje = "NO existe Nro de Venta" };
                    return Ok(resul);
                }

                //code 21
                // limite de consumo de personal
                string vender = "S";
                if (value.codTipoCliente.Equals("03"))
                {

                    var objLConsumo = await _repository.VentaCaja.GetCsLimiteConsumoPersonalPorCodPersonal(strCodCliente);
                    if (objLConsumo.ResultadoCodigo == -1)
                    {
                        return BadRequest(objLConsumo);
                    }

                    vender = objLConsumo.data.vender;
                    var montoconsumo = objLConsumo.data.montoconsumo;
                    var montolimite = objLConsumo.data.montolimite;
                    var fecha1 = objLConsumo.data.fecha1;
                    var fecha2 = objLConsumo.data.fecha2;

                    if (vender == "N")
                    {
                        var resul = new { exito = false, mensaje = $"El Consumo al CREDITO es mayor al límite_de_consumo <br><br>en el período del ${fecha1} al ${fecha2} <br>Monto Consumo (no incluye esta venta {montoconsumo} <br> Monto Limite de Consumo : ${montolimite}):" };
                        return Ok(resul);
                    }
                    else
                    {

                        var wMontoConsumo = objLConsumo.data.montoconsumo + value.montoTotal;

                        if (wMontoConsumo <= montolimite)
                        {
                            //beep graba
                        }
                        else
                        {
                            var resul = new { exito = false, mensaje = $"El Consumo al CREDITO es mayor al límite_de_consumo <br> en el período del {fecha1} al ${fecha2} <br> Monto Consumo (incluye esta venta): ${montoconsumo} <br> Monto Limite de Consumo : ${montolimite}" };
                            return Ok(resul);
                        }

                    }

                    foreach (var item in value.tipoPagos)
                    {

                    }

                }

                //code 24
                //Flag si puede generar FACTURA-Electronica pero no se envian a sunat. Por default si genera BOLETA-ELECTRONIA pero no se envía a Sunat.
                string wFlagGeneraFACTURA = string.Empty;
                var objFlgGENERAFACTURA = await _repository.Tabla.GetTablasClinicaPorFiltros("EFACT_FLAG_GENERAFACTURA", string.Empty, 0, 0, 5); //Electronico Enviar o no a SUNAT
                wFlagGeneraFACTURA = objFlgGENERAFACTURA.data.codigo;
                if (value.tipoComprobante.Substring(0, 1) == "F" && wFlagGeneraFACTURA == "N")
                {
                    var resul = new { exito = false, mensaje = "En estos momentos no se puede generar FACTURAS." };
                    return Ok(resul);
                }

                // code 28

                var objectGetAllTC = await _repository.TipoCambio.GetObtieneTipoCambio();

                if (objectGetAllTC.ResultadoCodigo == -1)
                {
                    return BadRequest(objectGetAllTC);
                }

                decimal tipoDeCambio = 0;

                foreach (var item in objectGetAllTC.dataList)
                {
                    value.tipoCambioVenta = item.Rate;
                    tipoDeCambio = item.Rate;
                }

                if (tipoDeCambio <= 0) return Ok(new { exito = false, mensaje = "NO TIENE CONFIGURADO EL TIPO DE CAMBIO" });

                var setDatos = value.RetornaComprobanteCabecera();

                var objComprobante = await _repository.VentaCaja.ComprobantesRegistrar(setDatos, value.tipoComprobante.Substring(0, 1), value.correo, value.codTipoAfectacionIgv, value.wFlg_electronico, value.maquina);

                if (objComprobante.IdRegistro == 0)
                {
                    return Ok(new { exito = true, comprobante = objComprobante.data, mensaje = $"SE REGISTRO CORRECTAMENTE EL PAGO: {objComprobante.data}" });
                }
                else
                {
                    return Ok(new { exito = false, mensaje = "SE GENERO UN ERROR AL MOMENTO DE GUARDAR EL PAGO: " + objComprobante.ResultadoDescripcion });
                }



            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetObtener()
        {
            var objectGetAll = await _repository.VentaCaja.GetCorrelativoConsulta();
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.data);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCorrelativoConsulta()
        {
            var objectGetAll = await _repository.VentaCaja.GetCorrelativoConsulta();
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }
            return Ok(objectGetAll.data);
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GenerarPreVistaPrint(string codventa, string maquina, int idusuario, int orden)
        {
            string archivoImg = string.Empty;
            var objConf = await _repository.Tabla.GetListTablaClinicaPorFiltros("EFACT_PARAMETROSWSTCI", "", 0, 0, 7);
            foreach (var item in objConf.dataList)
            {
                if (item.codigo.Trim().Equals("TEMP")) archivoImg = item.nombre;
            }

            var objectGetById = await _repository.VentaCaja.GenerarPreVistaPrint(codventa, maquina, archivoImg, idusuario, orden);
            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf", codventa + ".pdf");
            return pdf;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ComprobanteElectrValida(string codComprobante)
        {

            try
            {
                var objConf = await _repository.Tabla.GetTablasClinicaPorFiltros("EFACT_PARAMETROSWSTCI", "", 0, 0, 7);
                if (objConf.data == null) throw new ArgumentException("NO EXISTE EN A CONFIGURACION -> EFACT_PARAMETROSWSTCI");

                var objValida = await _repository.VentaCaja.GetComprobanteElectroncioVB("001", codComprobante, "", "L", "", 4);
                if (objValida.data == null) throw new ArgumentException("EL COMPROBANTE NO ES ELECTRONICO / NO EXISTE.");
                if (!objValida.data.flg_electronico.Equals("1")) throw new ArgumentException("EL COMPROBANTE NO ES ELECTRONICO.");
                if (!objValida.data.obtener_pdf.Equals("S")) throw new ArgumentException("AUN NO PUEDE IMPRIMIR COMPROBANTE ELECTRONICO. " +
                    " ESTADO CDR: " + objValida.data.nombreestado_cdr +
                    " Se Otorgó?: " + objValida.data.nombreestado_otorgamiento);

                var objExiste = _repository.VentaCaja.GetComprobanteElectroncioCodVenta(codComprobante, 0, 0, "", "", "", "", "", "", 1);
                if (objExiste.Result.data == null) throw new ArgumentException("No se puede obtener el Código del Comprobante Electrónico.\n Tabla comprobantes_electronicos  Cod. Comprobante PK: " + codComprobante);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PreVistaValida(string codComprobante)
        {

            try
            {
                var objCPE = await _repository.VentaCaja.GetComprobanteElectroncioCodVenta(codComprobante, 0, 0, "", "", "", "", "", "", 1);
                if (objCPE.data.codcomprobante == null) throw new ArgumentException("El comprobante que esta intentado obtener no es electrónico.");
                if (objCPE.data.codigobarra.Length == 0)
                {

                    ENPeticionInformacionComprobante modelo = new ENPeticionInformacionComprobante()
                    {
                        RucEmisor = objCPE.data.ruc_emisor,
                        TipoComprobante = objCPE.data.tipo_comprobante,
                        serie = objCPE.data.codcomprobantee.Substring(0, 4),
                        numero = Convert.ToInt32(objCPE.data.codcomprobantee.Substring(4)).ToString(),
                        CodigoHash = false,
                        CodigoBarra = true
                    };

                    Byte[] xJpgArchivo = new Byte[0];

                    string xRuta = string.Empty;
                    string pRutaArchivoJPG = string.Empty;
                    string xUrlWebService = string.Empty;

                    var objConf = await _repository.Tabla.GetListTablaClinicaPorFiltros("EFACT_PARAMETROSWSTCI", "", 0, 0, 7);
                    foreach (var item in objConf.dataList)
                    {
                        if (item.codigo.Trim().Equals("TEMP")) pRutaArchivoJPG = item.nombre;
                        if (item.codigo.Trim().Equals("WS")) xUrlWebService = item.nombre;
                    }

                    var xUrlWS = new System.ServiceModel.EndpointAddress(xUrlWebService);
                    var binding = new System.ServiceModel.BasicHttpBinding();
                    WSComprobanteSoapClient client = new WSComprobanteSoapClient(binding, xUrlWS);
                    string xRptaCadena = string.Empty;
                    var result = client.ConsultarInformacionComprobante(modelo);
                    if (result != null)
                    {
                        if (result.codigorespuesta.Equals("0"))
                        {

                            foreach (var item in result.respuesta)
                            {
                                xJpgArchivo = item.Codigobarra;
                                xRuta = pRutaArchivoJPG + codComprobante + ".jpg";
                                System.IO.File.WriteAllBytes(xRuta, xJpgArchivo);
                            }

                            var respUpd = await _repository.VentaCaja.ComprobanteElectronicoUpd("codigobarra", "", "", xJpgArchivo, codComprobante);
                            if (respUpd.ResultadoCodigo == -1)
                            {
                                return BadRequest(respUpd);
                            }
                            else
                            {
                                return Ok(respUpd);
                            }

                        }
                    }
                    else
                    {

                        return BadRequest("TCI: Error al consultar el Web Service");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> ComprobanteElectronicoPrint(string codComprobante)
        {


            string xRucEmisor = string.Empty, xUrlWebService = string.Empty;

            var objConf = await _repository.Tabla.GetListTablaClinicaPorFiltros("EFACT_PARAMETROSWSTCI", "", 0, 0, 7);
            foreach (var item in objConf.dataList)
            {
                if (item.codigo.Trim().Equals("RUC")) xRucEmisor = item.nombre;
                if (item.codigo.Trim().Equals("WS")) xUrlWebService = item.nombre;
            }

            var objExiste = await _repository.VentaCaja.GetComprobanteElectroncioCodVenta(codComprobante, 0, 0, "", "", "", "", "", "", 1);

            var xUrlWS = new System.ServiceModel.EndpointAddress(xUrlWebService);
            var binding = new System.ServiceModel.BasicHttpBinding();
            WSComprobanteSoapClient client = new WSComprobanteSoapClient(binding, xUrlWS);

            ENPeticion modelo = new ENPeticion()
            {
                IndicadorComprobante = 1,
                Ruc = xRucEmisor,
                Serie = codComprobante.Substring(0, 4),
                Numero = Convert.ToInt32(codComprobante.ToString().Substring(4)).ToString(),
                TipoComprobante = objExiste.data.tipo_comprobante
            };

            string xRptaCadena = string.Empty;
            var result = client.Obtener_PDF(modelo, ref xRptaCadena);
            var pdf = File(result.ArchivoPDF, "applicacion/pdf", codComprobante + ".pdf");
            return pdf;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// 
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Anular([FromBody] DtoComprobanteDelete value)
        {

            var fechaHoy = new DateTime();

            try
            {
                if (value.codComprobante == null || value.codComprobante == string.Empty)
                {
                    return BadRequest("comprobante no puede ser nulo");
                }

                var codComprobante = value.codComprobante.Trim();
                if (codComprobante == string.Empty || codComprobante.Length != 11) return BadRequest("Comprobante Incorrecto, Verifique porfavor!");

                var objList = await _repository.Comprobante.GetComprobanteConsulta(codComprobante, 0, 0, 0);
                if (objList.dataList.Count() == 0) return BadRequest("No existe comprobante");
                var obj = objList.dataList.FirstOrDefault();

                if (obj.estado.Equals("X")) return BadRequest("El Comprobante YA Fue Anulado Anteriormente");
                if (!obj.numeroplanilla.Equals("")) return BadRequest("El comprobante esta incluido en la planilla");
                if (obj.fechacancelacion.Value.Month != fechaHoy.Month) return BadRequest("No se puede anular porque es del mes pasado");

                //EFACT- NO debe anular comprobantes que tienen notas activas.
                var objListVb = await _repository.VentaCaja.GetComprobanteElectroncioVB("001", codComprobante, "", "L", "", 5);
                if (objListVb.dataList.Count() == 0) return BadRequest("Error...");

                var objvb = objListVb.dataList.FirstOrDefault();
                if (!objvb.flg_electronico.Equals("1")) return BadRequest("Ud. Eliminará un Comprobante físico");
                if (!objvb.anular.Equals("S")) return BadRequest($"No puede anular este comprobante. {objvb.mensaje} Regla: Tener CDR-Rechazado, Tener envío a baja aceptado.");

                var objCPDelete = await _repository.Comprobante.ComprobanteDelete(codComprobante);
                if (!objCPDelete.ResultadoCodigo.Equals(0)) return BadRequest(objCPDelete.ResultadoDescripcion);

                var objCPUpdateUser = await _repository.Comprobante.ComprobantesUpdate("coduser", codComprobante, "coduser01");
                if (!objCPUpdateUser.ResultadoCodigo.Equals(0)) return BadRequest(objCPUpdateUser.ResultadoDescripcion);

                var objCPUpdateCentro = await _repository.Comprobante.ComprobantesUpdate("codcentro", codComprobante, "wCodCentro01");
                if (!objCPUpdateUser.ResultadoCodigo.Equals(0)) return BadRequest(objCPUpdateUser.ResultadoDescripcion);

                var objcp = await _repository.Comprobante.GetComprobanteConsulta(codComprobante, 0, 0, 0);
                if (objcp.dataList.Count() == 0) return BadRequest("se genero un error al traer el comprobante");
                var objcpfirs = objList.dataList.FirstOrDefault();

                return Ok(new { estado = objcpfirs.estado, mensaje = $"SE ANULO CORRECTAMENTE {codComprobante}" });


            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GenerarPagoBot([FromBody] DtoOrdenPagoBotRequest value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var datos = value.RetornaMdsynPagos();

                #region Configuracion de SYNAPSIS

                /*(I) Configuracion de SYNAPSIS*/
                //Synapsis_ApiKey
                var objResultApiKey = _repository.Tabla.GetListTablaClinicaPorFiltros("MEDISYN_SYNAPSIS_APIKEY", "01", 50, 1, -1);
                var objApiKey = objResultApiKey.Result.dataList.FirstOrDefault();
                string Synapsis_ApiKey = objApiKey.nombre.Trim();

                //Synapsis_SignatureKey
                var objResultApiSecre = _repository.Tabla.GetListTablaClinicaPorFiltros("MEDISYN_SYNAPSIS_SECRETKEY", "01", 50, 1, -1);
                var objApiSecre = objResultApiSecre.Result.dataList.FirstOrDefault();
                string Synapsis_SignatureKey = objApiSecre.nombre.Trim();

                //Synapsis_Ws_Url
                var objResultApiLink = _repository.Tabla.GetListTablaClinicaPorFiltros("MEDISYN_SYNAPSIS_URL", "01", 50, 1, -1);
                var objApiLink = objResultApiLink.Result.dataList.FirstOrDefault();
                string Synapsis_Ws_Url = objApiLink.nombre.Trim();
                /*(F) Configuracion de SYNAPSIS*/

                #endregion

                //tipoPago = 2 (farmacia)
                var result = _repository.SynapsisWSRepository.fnGenerarOrdenPagoBot(datos, 2, value.idUsuario, Synapsis_ApiKey, Synapsis_SignatureKey, Synapsis_Ws_Url);
                //if (result.data.responseOrderApi.success) {
                if (result.ResultadoCodigo == 0)
                {
                    //return Ok(result.data.responseOrderApi);
                    return Ok(result);
                }
                else
                {
                    //result.ResultadoDescripcion = "Hubo un error al procesar la orden del pago.\n Intentelo más tarde.\n" + result.data.responseOrderApi.message.text;
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                var result = new ResultadoTransaccion<BE_SYNAPSIS_ResponseOrderApiResult>();
                result.ResultadoDescripcion = ex.Message;
                return BadRequest(result);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="codventa"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExisteVentaAnuladaPorCodVenta([FromQuery] string codventa)
        {
            var existeAnulacion = await _repository.VentaCaja.GetExisteVentaAnuladaPorCodVenta(codventa);
            return Ok(existeAnulacion);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ide_pagos_bot"></param>
        /// <param name="ide_mdsyn_reserva"></param>
        /// <param name="ide_correl_reserva"></param>
        /// <param name="cod_liquidacion"></param>
        /// <param name="cod_venta"></param>
        /// <param name="orden"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerPagoBot([FromQuery] long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden)
        {
            cod_liquidacion = cod_liquidacion is null ? string.Empty : cod_liquidacion;
            var objectGetAll = await _repository.VentaCaja.GetMdsynPagosConsulta(ide_pagos_bot, ide_mdsyn_reserva, ide_correl_reserva, cod_liquidacion, cod_venta, orden);
            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(new DtoObtenerPagoBotResponse().RetornarObtenerPagoBot(objectGetAll.data));

        }




    }
}
