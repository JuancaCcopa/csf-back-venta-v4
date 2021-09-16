using Net.Business.Entities;
using Net.Connection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Net.TCI;
using MSXML2;

namespace Net.Data
{
    public class ComprobanteElectronicoRepository : RepositoryBase<BE_ComprobanteElectronico>, IComprobanteElectronicoRepository
    {
        private readonly string _cnx;
        private readonly string _cnx_logistica;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        private readonly IConfiguration _configuration;
        const string DB_ESQUEMA = "";
        const string SP_GET = DB_ESQUEMA + "Sp_ComprobantesElectronicos_ConsultaVB";
        const string SP_GET_COMPROBANTEELECTRONICO = DB_ESQUEMA + "VEN_ComprobantesElectronicosLogXmlGet";
        const string SP_GET_NOTAELECTRONICO = DB_ESQUEMA + "VEN_NotaElectronicaLogXmlGet";
        const string SP_UPD_COMPROBANTEELECTRONICO = DB_ESQUEMA + "VEN_ComprobantesElectronicosUpd";

        public ComprobanteElectronicoRepository(IConnectionSQL context, IConfiguration configuration)
           : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlClinica");
            _cnx_logistica = configuration.GetConnectionString("cnnSqlLogistica");
            _aplicacionName = this.GetType().Name;
            _configuration = configuration;
        }

        public async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetListComprobanteElectronicoPorFiltro(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {

                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    using (SqlCommand cmd = new SqlCommand(SP_GET, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codempresa", codempresa));
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante_e", codcomprobante_e));
                        cmd.Parameters.Add(new SqlParameter("@codsistema", codsistema));
                        cmd.Parameters.Add(new SqlParameter("@tipocomp_sunat", tipocomp_sunat));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        var response = new List<BE_ComprobanteElectronico>();

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
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
        public async Task<int> Registrar(string codcomprobante, string tipoCodigo_BarraHash, string tipoOtorgamiento, string Xml)
        {
            string wCuadre;
            string wCodComprobanteE;
            string wTipoComp_TCI;
            string wTipoCodigo;
            string wOtorgar;
            string[] respuesta = new string[1];
            string secuencia = "\r";

            wCodComprobanteE = "";
            wTipoComp_TCI = "";
            wTipoCodigo = tipoCodigo_BarraHash; // '"1" 'Se envía "0" si se desea obtener el código de barras; se envía "1" si se desea obtener el código hash.
            wOtorgar = tipoOtorgamiento;        //'"1" 'Se envía "0" si se otorgará manualmente, se envía "1" si se otorgará automáticamente
            wCuadre = "";

            //Datos de comprobante
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();

            if (codcomprobante.Substring(0, 1) == "F" | codcomprobante.Substring(0, 1) == "B")
            {
                vResultadoTransaccion = await GetComprobantesElectronicosXml("", "", "", 0);
            }
            else if (codcomprobante.Substring(0, 1) == "C" | codcomprobante.Substring(0, 1) == "D")
            {
                vResultadoTransaccion = await GetNotaElectronicaLogXml("", 0);
            }

            if (vResultadoTransaccion.IdRegistro == -1)
            {
                Xml = "";
                return -4; //'Error en Stored
            }

            List<BE_ComprobanteElectronico> response = (List<BE_ComprobanteElectronico>)vResultadoTransaccion.dataList;

            if (response.Count > 0)
            {
                wCodComprobanteE = response[0].ref_codcomprobantee;
                wTipoComp_TCI = response[0].tipocomp_tci; //'Regla_TCI: "Factura", "Boleta"
                wCuadre = response[0].cuadre;
                if (wCuadre == "")
                {
                    Xml = "";
                    return -1; //'Detalle tiene Negativo
                }
            }
            else
            {
                Xml = "";
                return -2; //'Comprobante no existe o No hay detalle
            }

            //' ------------------- Invocación del Web Service
            string XML_Cabecera;

            FacturacionElectronica _facturacionElectronica = new FacturacionElectronica();

            XML_Cabecera = _facturacionElectronica.GetComprobateElectronicoXml(response);

            if (XML_Cabecera != "")
            {
                //'I-TCI
                string strSOAPAction;
                string strURL;
                string Metodo;

                strURL = "http://200.106.52.10/WS_TCI/Service.asmx?wsdl";
                strSOAPAction = "http://tempuri.org/Registrar";

                //'I-Ali-csf
                Metodo = "";
                Metodo = Metodo + "<?xml version= " + "1.0" + " encoding= " + "utf-8" + "?> " + secuencia;
                Metodo = Metodo + "<soap:Envelope xmlns:xsi=" + "http://www.w3.org/2001/XMLSchema-instance" + " xmlns:xsd=" + "http://www.w3.org/2001/XMLSchema" + " ";
                Metodo = Metodo + " xmlns:soap=" + "http://schemas.xmlsoap.org/soap/envelope/" + ">" + secuencia +
                        "<soap:Body>" + secuencia +
                            "<Registrar xmlns=" + "http://tempuri.org/" + ">" + secuencia +
                                "<oGeneral>" + secuencia + XML_Cabecera + "</oGeneral>" + secuencia +
                                "<oTipoComprobante>" + wTipoComp_TCI + "</oTipoComprobante>" + secuencia +
                                "<Cadena></Cadena>" + secuencia +
                                "<TipoCodigo>" + wTipoCodigo + "</TipoCodigo>" + secuencia +
                                "<CogigoBarras></CogigoBarras>" + secuencia +
                                "<CogigoHash></CogigoHash>" + secuencia +
                                "<Otorgar>" + wOtorgar + "</Otorgar>" + secuencia +
                                "<IdComprobanteCliente>0</IdComprobanteCliente>" + secuencia +
                            "</Registrar>" + secuencia +
                        "</soap:Body>" + secuencia +
                    "</soap:Envelope>";
                //'F-Ali-csf
                Xml = Metodo;
            }
            else
            {
                Xml = "";
                return -3; // 'Error al construir XML
            }

            return 0;
        }


        private async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetComprobantesElectronicosXml(string codcomprobante, string xcoduser, string maquina, int orden)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_COMPROBANTEELECTRONICO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@xcoduser", xcoduser));
                        cmd.Parameters.Add(new SqlParameter("@maquina", maquina));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        var response = new List<BE_ComprobanteElectronico>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
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

        public async Task<ResultadoTransaccion<string>> EnviarComprobanteElectronica(string tipocomprobante, string comprobante)
        {
            string tipoCodigoBarraHash = string.Empty;
            string otorgamiento = string.Empty;
            string strURL = string.Empty;

            ResultadoTransaccion<string> resultadoTransaccionError = new ResultadoTransaccion<string>();

            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();

            SerieRepository serieRepository = new SerieRepository(context, _configuration);
            ResultadoTransaccion<BE_SerieConfig> resultadoTransSerie = new ResultadoTransaccion<BE_SerieConfig>();

            //resultadoTransSerie = await serieRepository.GetCorrelativo();
            //resultadoTransSerie = await serieRepository.GetListConfigDocumentoPorNombreMaquina(nombreMaquina);//Pendiente por modificar

            if (resultadoTransSerie.IdRegistro == 0)
            {
                switch (tipocomprobante)
                {
                    case "C":
                        switch (comprobante.Substring(0, 1))
                        {
                            case "B":
                                otorgamiento = resultadoTransSerie.data.flg_otorgarcb.ToString();
                                break;
                            case "F":
                                otorgamiento = resultadoTransSerie.data.flg_otorgarcf.ToString();
                                break;
                        }
                        break;
                    case "D":
                        switch (comprobante.Substring(0, 1))
                        {
                            case "B":
                                otorgamiento = resultadoTransSerie.data.flg_otorgardb.ToString();
                                break;
                            case "F":
                                otorgamiento = resultadoTransSerie.data.flg_otorgardf.ToString();
                                break;
                        }
                        break;
                }
            }

            ResultadoTransaccion<BE_Tabla> resultadoTransaTabla = null;
            TablaRepository TablaRepository = new TablaRepository(context, _configuration);

            resultadoTransaTabla = new ResultadoTransaccion<BE_Tabla>();

            resultadoTransaTabla = await TablaRepository.GetTablasTCIWebService("EFACT_TCI_WS");

            if (resultadoTransaTabla.IdRegistro == 0)
            {
                strURL = resultadoTransaTabla.data.nombre;
            }

            resultadoTransaTabla = new ResultadoTransaccion<BE_Tabla>();
            resultadoTransaTabla = await TablaRepository.GetTablaLogisticaPorFiltros("EFACT_TIPOCODIGO_BARRAHASH", "01", 50, 1, -1);

            if (resultadoTransaTabla.IdRegistro == 0)
            {
                tipoCodigoBarraHash = resultadoTransaTabla.data.valor.ToString();
            }
            if (tipoCodigoBarraHash == "" || tipoCodigoBarraHash != "0" || tipoCodigoBarraHash != "1")
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = "Solicite asignar se se imprimirá Codigo de Barras o Codigo Hash.";
            }


            //VentasNotaRepository ventasNotaRepository = new VentasNotaRepository(context, _configuration);
            //ResultadoTransaccion<BE_VentasNota> resultadoTransaNota = new ResultadoTransaccion<BE_VentasNota>();

            //resultadoTransaNota = await ventasNotaRepository.GetNotaPorFiltro(wCodcomprobante, 0, 0, 4, "");

            //if (resultadoTransaNota.IdRegistro == 0)
            //{
            //    wCodNotaE = resultadoTransaNota.data.codnota;
            //}

            //if (wCodNotaE.Trim().Equals(""))
            //{
            //    vResultadoTransaccion.IdRegistro = -1;
            //    vResultadoTransaccion.ResultadoCodigo = -1;
            //    vResultadoTransaccion.ResultadoDescripcion = "Error..No generó la nota electronica.";
            //}

            string wCuadre = string.Empty;
            string wTipoComp_TCI = string.Empty;
            string TipoComp_TCI = string.Empty;


            resultadoTransaTabla = new ResultadoTransaccion<BE_Tabla>();
            resultadoTransaTabla = await TablaRepository.GetTablaLogisticaPorFiltros("EFACT_TIPOCOMP_FORCALL_WS_TCI", comprobante.Substring(0, 1), 50, 1, -1);

            if (resultadoTransaTabla.IdRegistro == 0)
            {
                TipoComp_TCI = resultadoTransaTabla.data.nombre;
            }

            ResultadoTransaccion<BE_ComprobanteElectronico> resultadoTransaccionComprobanteElectronico = new ResultadoTransaccion<BE_ComprobanteElectronico>();

            if (comprobante.Substring(0, 1) == "F" | comprobante.Substring(0, 1) == "B")
            {
                resultadoTransaccionComprobanteElectronico = await GetComprobantesElectronicosXml(comprobante, 0);
            }
            else if (comprobante.Substring(0, 1) == "C" | comprobante.Substring(0, 1) == "D")
            {
                resultadoTransaccionComprobanteElectronico = await GetNotaElectronicaLogXml(comprobante, 0);
            }

            if (resultadoTransaccionComprobanteElectronico.IdRegistro == -1)
            {
                resultadoTransaccionError.IdRegistro = -1;
                resultadoTransaccionError.ResultadoCodigo = -1;
                resultadoTransaccionError.ResultadoDescripcion = "Error en el procedimiento almacenado.";
            }

            List<BE_ComprobanteElectronico> response = (List<BE_ComprobanteElectronico>)resultadoTransaccionComprobanteElectronico.dataList;

            if (response.Count > 0)
            {
                wTipoComp_TCI = response[0].tipocomp_tci; //'Regla_TCI: "Factura", "Boleta"
                wCuadre = response[0].cuadre;

                if (string.IsNullOrEmpty(wCuadre))
                {
                    resultadoTransaccionError.IdRegistro = -1;
                    resultadoTransaccionError.ResultadoCodigo = -1;
                    resultadoTransaccionError.ResultadoDescripcion = "El comprobante tiene detalle negativo.";
                }
            }
            else
            {
                resultadoTransaccionError.IdRegistro = -1;
                resultadoTransaccionError.ResultadoCodigo = -1;
                resultadoTransaccionError.ResultadoDescripcion = "El comprobante no existe o no tiene detalle negativo.";
            }

            //'--Construir XML
            ComprobanteElectronicaTCIXml comprobanteElectronicaTCIXml = new ComprobanteElectronicaTCIXml();

            string Xml = string.Empty;

            var resultado = await comprobanteElectronicaTCIXml.GetXML(response, wTipoComp_TCI, tipoCodigoBarraHash, otorgamiento, Xml);

            if (resultado != 0)
            {
                resultadoTransaccionError.IdRegistro = -1;
                resultadoTransaccionError.ResultadoCodigo = -1;
                resultadoTransaccionError.ResultadoDescripcion = "Error al construir XML.";
            }
            else
            {
                //' ------------------- Invocación del Web Service
                string strSOAPAction = "";

                strSOAPAction = "http://tempuri.org/Registrar";

                var parser = new DOMDocument30();
                //'parser.async = False     'AGARCIA: NO Usar ESTO PORQUE da error HTML, error = 502 = problemas de gateway ..
                //'cargar el código SOAP
                parser.loadXML(Xml);

                if (!parser.loadXML(Xml))
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "Error al construir XML. Comuniquese con el area de sistemas";
                }

                //'Indicar el parámetro a enviar - AG: Lo que hacemos es asignar el parámetro que queremos pasarle a la función Registrar
                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/oTipoComprobante").text = TipoComp_TCI;   //'TCI-Regla: enviar "Factura","Boleta","NotaCredito","NotaDebito"
                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/TipoCodigo").text = tipoCodigoBarraHash;  //'TCI "1"
                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/Otorgar").text = otorgamiento;            //'TCI "1"
                parser.selectSingleNode("/soap:Envelope/soap:Body/Registrar/IdComprobanteCliente").text = "0";        //'TCI-Regla: enviar CodComprobante en int; 08/06/2015:No se enviara porq TCI no soporta(INTEGER) series mayores a 214; Mid(wCodComprobante, 2, 10)

                string strXml;
                strXml = parser.xml;

                //'I-Call PostWebservice y Leer rpta de TCI - usando code de csf
                DOMDocument30 xmlResponse = new DOMDocument30();
                string wResponseXML = string.Empty;
                string wResponseTXT = string.Empty;
                string wCodigoHash = string.Empty;
                string wCodigoBarra = string.Empty;
                string wCadenaRpta = string.Empty;

                if (comprobanteElectronicaTCIXml.InvokeWebServiceTCI(strXml.Trim(), strSOAPAction, strURL, xmlResponse))
                {
                    wResponseXML = xmlResponse.xml;
                    wResponseTXT = xmlResponse.text;

                    //'I-Leer parametro de salida Retorno(true/false) para saber si se registró en SUNAT - (Ref: AgregarPacientesCSFaTisal)
                    string ResultStatus;
                    string wStatus;
                    DOMDocument xml_document;
                    xml_document = new DOMDocument();

                    xml_document.loadXML(wResponseXML);
                    ResultStatus = "/soap:Envelope/soap:Body/RegistrarResponse/";
                    wStatus = xml_document.selectSingleNode(ResultStatus + "RegistrarResult").text;// En VB6: xml_document.selectSingleNode devuelve boleano a pesar de que es text
                                                                                                   //'MsgBox ("Status : " & xml_document.selectSingleNode(ResultStatus & "RegistrarResult").Text), , "Metodo_Registrar " & wCodComprobante & " - "
                                                                                                   //'F-Leer parametro de salida Retorno(true/false)

                    //'2.I-Procedimiento segun wStatus
                    if (wStatus == "")
                    {
                        //'MsgBox "True - Se registró en SUNAT", , "Método Registrar " & wCodComprobante
                        //'IMPORTANTE: NO HACER zClinicac.RollbackTrans PORQUE YA ESTA REGISTRADO EN SUNAT
                        vResultadoTransaccion = await ModificarComprobanteElectronico("fecha_registro_rpta", "", "", comprobante);
                        vResultadoTransaccion = await ModificarComprobanteElectronico("tipo_otorgamiento", otorgamiento, "", comprobante);
                        vResultadoTransaccion = await ModificarComprobanteElectronico("xml_registro", "", strXml.Trim(), comprobante);
                        wStatus = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<RegistrarResult>", "</RegistrarResult>");
                        wCadenaRpta = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<Cadena>", "</Cadena>");
                        vResultadoTransaccion = await ModificarComprobanteElectronico("observacion_registro", wStatus + "; " + wCadenaRpta.Substring(0, 3980), "", comprobante);
                        wCodigoHash = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<CodigoHash>", "</CodigoHash>");
                        wCodigoBarra = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<CodigoBarras>", "</CodigoBarras>");

                        //'zFarmacia.Sp_ComprobantesElectronicos_Update "codigohash", wCodigoHash, "", wCodcomprobante
                        //--> zEfact.ConvertirCodigoBarraJPG wCodcomprobante, wCodigoBarra, zFarmacia2
                    }
                    else
                    {
                        //'MsgBox "False - Error al Registrar", , "Metodo_Registrar " & wCodComprobante
                        //'wCadena = xml_document.selectSingleNode(ResultStatus & "Cadena").Text
                        wCadenaRpta = comprobanteElectronicaTCIXml.Leer_ResponseXML(wResponseXML, "<Cadena>", "</Cadena>");
                        //EnviarCorreo_Error wCodcomprobante, wCadenaRpta

                        vResultadoTransaccion.IdRegistro = -1;
                        vResultadoTransaccion.ResultadoCodigo = -1;
                        vResultadoTransaccion.ResultadoDescripcion = "Metodo enviar " + comprobante;
                    }
                }
                else
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = "Error al invocar WebService : InvokeWebService_CSF " + comprobante;
                }
            }

            return resultadoTransaccionError;
        }

        private async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetComprobantesElectronicosXml(string codcomprobante, int orden)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx_logistica))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_COMPROBANTEELECTRONICO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codcomprobante", codcomprobante));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        var response = new List<BE_ComprobanteElectronico>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
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

        private async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetNotaElectronicaLogXml(string codnota, int orden)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx_logistica))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_NOTAELECTRONICO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codnota", codnota));
                        cmd.Parameters.Add(new SqlParameter("@orden", orden));

                        conn.Open();

                        var response = new List<BE_ComprobanteElectronico>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ComprobanteElectronico>)context.ConvertTo<BE_ComprobanteElectronico>(reader);
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

        private async Task<ResultadoTransaccion<BE_ComprobanteElectronico>> ModificarComprobanteElectronico(string campo, string nuevoValor, string XML, string codigo)
        {
            ResultadoTransaccion<BE_ComprobanteElectronico> vResultadoTransaccion = new ResultadoTransaccion<BE_ComprobanteElectronico>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx_logistica))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_UPD_COMPROBANTEELECTRONICO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add(new SqlParameter("@campo", campo));
                        cmd.Parameters.Add(new SqlParameter("@nuevovalor", nuevoValor));
                        cmd.Parameters.Add(new SqlParameter("@XML", XML));
                        cmd.Parameters.Add(new SqlParameter("@codigo", codigo));

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        vResultadoTransaccion.IdRegistro = 0;
                        vResultadoTransaccion.ResultadoCodigo = 0;
                        vResultadoTransaccion.ResultadoDescripcion = "Comprobante electrónico procesado con éxito.";
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
