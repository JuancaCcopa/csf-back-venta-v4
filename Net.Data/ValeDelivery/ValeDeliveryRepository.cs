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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace Net.Data
{
    public class ValeDeliveryRepository : RepositoryBase<BE_ValeDelivery>, IValeDeliveryRepository
    { 
        private readonly string _cnx;

        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_VALEDELIVERY_POR_ATENCION = DB_ESQUEMA + "VEN_ValeDeliveryGet";
        const string SP_GET_VALEDELIVERY_POR_RANGOFECHA = DB_ESQUEMA + "VEN_ValeDeliveryPorRangoFechaGet";

        const string SP_INSERT_VALEDELIVERY = DB_ESQUEMA + "VEN_ValeDeliveryIns";
        const string SP_UPDATE_VALEDELIVERY = DB_ESQUEMA + "VEN_ValeDeliveryUpd";

        const string SP_GET_VALEDELIVERY_POR_RANGO_FECHA = DB_ESQUEMA + "VEN_ValeDeliveryPorRangoFechaGet";
        const string SP_GET_VALEDELIVERY_AGRUPAGO_ESTADO_POR_RANGO_FECHA = DB_ESQUEMA + "VEN_ValeDeliveryAgrupoEstadoPorRangoFechaGet";
        const string SP_GET_VALEDELIVERY_MEDECIMANTO_POR_CODIGO_ATENCION = DB_ESQUEMA + "VEN_ValeDeliveryMedicamentosPorCodigoAtencionGet";

        public ValeDeliveryRepository(IConnectionSQL context, IConfiguration configuration)
            : base(context)
        {
            _aplicacionName = this.GetType().Name;
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
        }

        public async Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryPorCodAtencion(string codatencion)
        {
            ResultadoTransaccion<BE_ValeDelivery> vResultadoTransaccion = new ResultadoTransaccion<BE_ValeDelivery>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;
            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ValeDelivery>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VALEDELIVERY_POR_ATENCION, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@codatencion", codatencion));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ValeDelivery>)context.ConvertTo<BE_ValeDelivery>(reader);
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

        public async Task<ResultadoTransaccion<BE_ValeDelivery>> RegistrarValeDelivery(BE_ValeDeliveryXml value)
        {
            ResultadoTransaccion<BE_ValeDelivery> vResultadoTransaccion = new ResultadoTransaccion<BE_ValeDelivery>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_INSERT_VALEDELIVERY, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        SqlParameter oParam = new SqlParameter("@idvaledelivery", value.idvaledelivery);
                        oParam.SqlDbType = SqlDbType.Int;
                        oParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(oParam);

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

                        vResultadoTransaccion.IdRegistro = (int)oParam.Value;
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

        public async Task<ResultadoTransaccion<BE_ValeDelivery>> ModificarValeDelivery(BE_ValeDeliveryXml value)
        {
            ResultadoTransaccion<BE_ValeDelivery> vResultadoTransaccion = new ResultadoTransaccion<BE_ValeDelivery>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            using (SqlConnection conn = new SqlConnection(_cnx))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand(SP_UPDATE_VALEDELIVERY, conn))
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

        public async Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryPorRangoFecha(DateTime fechaInicio, DateTime fechaFinal)
        {
            ResultadoTransaccion<BE_ValeDelivery> vResultadoTransaccion = new ResultadoTransaccion<BE_ValeDelivery>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            fechaInicio = Utilidades.GetFechaHoraInicioActual(fechaInicio);
            fechaFinal = Utilidades.GetFechaHoraFinActual(fechaFinal);

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ValeDelivery>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VALEDELIVERY_POR_RANGO_FECHA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechainicio", fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechafin", fechaFinal));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_ValeDelivery>)context.ConvertTo<BE_ValeDelivery>(reader);
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

        public async Task<ResultadoTransaccion<MemoryStream>> GetGenerarValeValeDeliveryReporte1Print(DateTime fechaInicio, DateTime fechaFinal)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.A4);
                // points to cm
                doc.SetMargins(20f, 20f, 15f, 15f);
                MemoryStream ms = new MemoryStream();
                PdfWriter write = PdfWriter.GetInstance(doc, ms);
                doc.AddAuthor("Grupo SBA");
                doc.AddTitle("Cliníca San Felipe");
                var pe = new PageEventHelper();
                write.PageEvent = pe;
                // Colocamos la fuente que deseamos que tenga el documento
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 5f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 6f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                ResultadoTransaccion<BE_ValeDelivery> resultadoTransaccion = await GetListValeDeliveryPorRangoFecha(fechaInicio, fechaFinal);

                List<BE_ValeDelivery> response = (List<BE_ValeDelivery>)resultadoTransaccion.dataList;

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var title = string.Format("REPORTE DE ENTREGAS", titulo);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta

                tbl = new PdfPTable(new float[] { 3f, 6f, 6f, 7f, 10f, 14f, 10f, 6f, 7f, 15f, 6f, 10f }) { WidthPercentage = 100 };

                c1 = new PdfPCell(new Phrase("Nº", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Código venta", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nº Atención", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha venta", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Apellidos y Nombre", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Dirección", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Distrito", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Referencia", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Teléfono", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Lugar de recojo", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Prioridad", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Estado delivery", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                tbl.AddCell(c1);

                var cont = 1;
                foreach (BE_ValeDelivery item in response)
                {

                    c1 = new PdfPCell(new Phrase(cont.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.codventa, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.codatencion, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase((item.fecharegistro == null ? null : DateTime.Parse(item.fecharegistro.ToString()).ToString("dd/MM/yyyy")), parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.nombrepaciente, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.direccion, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.distrito, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.referencia, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.telefono, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.lugarentrega, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.prioridad_1, parrafoNegro));
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.estadovd, parrafoNegro));
                    tbl.AddCell(c1);

                    cont++;
                }
                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = file;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryAgrupoEstadoPorRangoFecha(DateTime fechaInicio, DateTime fechaFinal)
        {
            ResultadoTransaccion<BE_ValeDelivery> vResultadoTransaccion = new ResultadoTransaccion<BE_ValeDelivery>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            fechaInicio = Utilidades.GetFechaHoraInicioActual(fechaInicio);
            fechaFinal = Utilidades.GetFechaHoraFinActual(fechaFinal);

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ValeDelivery>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VALEDELIVERY_AGRUPAGO_ESTADO_POR_RANGO_FECHA, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", fechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFinal", fechaFinal));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime? fecha = null;

                                if ((reader["fecharegistro"]) is DBNull) fecha = DateTime.Parse(reader["fecharegistro"].ToString());


                                var valeDelivery = new BE_ValeDelivery()
                                {
                                    fechaventa = fecha,
                                    atendido = ((reader["atendido"]) is DBNull) ? 0 : int.Parse(reader["atendido"].ToString()),
                                    noDeseaReceta = ((reader["noDeseaReceta"]) is DBNull) ? 0 : int.Parse(reader["noDeseaReceta"].ToString()),
                                    noSePudoContactar = ((reader["noSePudoContactar"]) is DBNull) ? 0 : int.Parse(reader["noSePudoContactar"].ToString()),
                                    pendiente = ((reader["pendiente"]) is DBNull) ? 0 : int.Parse(reader["pendiente"].ToString()),
                                    recogidoCSF = ((reader["recogidoCSF"]) is DBNull) ? 0 : int.Parse(reader["recogidoCSF"].ToString())
                                };

                                response.Add(valeDelivery);
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

        public async Task<ResultadoTransaccion<MemoryStream>> GetGenerarValeValeDeliveryReporte2Print(DateTime fechaInicio, DateTime fechaFinal)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.A4);
                // points to cm
                doc.SetMargins(20f, 20f, 15f, 15f);
                MemoryStream ms = new MemoryStream();
                PdfWriter write = PdfWriter.GetInstance(doc, ms);
                doc.AddAuthor("Grupo SBA");
                doc.AddTitle("Cliníca San Felipe");
                var pe = new PageEventHelper();
                write.PageEvent = pe;
                // Colocamos la fuente que deseamos que tenga el documento
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 10f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                ResultadoTransaccion<BE_ValeDelivery> resultadoTransaccion = await GetListValeDeliveryAgrupoEstadoPorRangoFecha(fechaInicio, fechaFinal);

                List<BE_ValeDelivery> response = (List<BE_ValeDelivery>)resultadoTransaccion.dataList;

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 10f, 80f, 10f }) { WidthPercentage = 100 };

                var title = string.Format("REPORTE DE ENTREGAS AGRUPADO POR ESTADO", titulo);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta

                tbl = new PdfPTable(new float[] { 15f, 10f, 10f, 17f, 21f, 10f, 17f }) { WidthPercentage = 100 };

                c1 = new PdfPCell(new Phrase("Fecha venta", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nº", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Atendido", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("No desea receta", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("No se puede contactar", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Pendiente", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Recogido CSF", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);

                var cont = 1;
                var contTot = 0;
                var atendido = 0;
                var noDeseaReceta = 0;
                var noSePudoContactar = 0;
                var pendiente = 0;
                var recogidoCSF = 0;

                foreach (BE_ValeDelivery item in response)
                {
                    contTot++;
                    atendido += item.atendido;
                    noDeseaReceta += item.noDeseaReceta;
                    noSePudoContactar += item.noSePudoContactar;
                    pendiente += item.pendiente;
                    recogidoCSF += item.recogidoCSF;

                    c1 = new PdfPCell(new Phrase((item.fechaventa == null ? null : DateTime.Parse(item.fechaventa.ToString()).ToString("dd/MM/yyyy")), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(cont.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.atendido.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.noDeseaReceta.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.noSePudoContactar.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.pendiente.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.recogidoCSF.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    cont++;
                }

                c1 = new PdfPCell(new Phrase("Total", parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(contTot.ToString(), parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(atendido.ToString(), parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(noDeseaReceta.ToString(), parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(noSePudoContactar.ToString(), parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(pendiente.ToString(), parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(recogidoCSF.ToString(), parrafoNegroNegrita)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = file;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }
            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryMedicamentosPorCodigoAtencion(int idvaledelivery)
        {
            ResultadoTransaccion<BE_ValeDelivery> vResultadoTransaccion = new ResultadoTransaccion<BE_ValeDelivery>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {
                    var response = new List<BE_ValeDelivery>();
                    using (SqlCommand cmd = new SqlCommand(SP_GET_VALEDELIVERY_MEDECIMANTO_POR_CODIGO_ATENCION, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@idvaledelivery", idvaledelivery));

                        conn.Open();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime? fecha;

                                if ((reader["fecharegistro"]) is DBNull)
                                {
                                    fecha = null;
                                }
                                else
                                {
                                    fecha = DateTime.Parse(reader["fecharegistro"].ToString());
                                }

                                var valeDelivery = new BE_ValeDelivery()
                                {
                                    codatencion = ((reader["codatencion"]) is DBNull) ? "" : reader["codatencion"].ToString(),
                                    idvaledelivery = ((reader["idvaledelivery"]) is DBNull) ? 0 : int.Parse(reader["idvaledelivery"].ToString()),
                                    fecharegistro = fecha,
                                    codigousuario = ((reader["codigousuario"]) is DBNull) ? "" : reader["codigousuario"].ToString(),
                                    nombreusuario = ((reader["nombreusuario"]) is DBNull) ? "" : reader["nombreusuario"].ToString(),
                                    nombrepaciente = ((reader["nombrepaciente"]) is DBNull) ? "" : reader["nombrepaciente"].ToString(),
                                    telefono = ((reader["telefono"]) is DBNull) ? "" : reader["telefono"].ToString(),
                                    celular = ((reader["celular"]) is DBNull) ? "" : reader["celular"].ToString(),
                                    direccion = ((reader["direccion"]) is DBNull) ? "" : reader["direccion"].ToString(),
                                    distrito = ((reader["distrito"]) is DBNull) ? "" : reader["distrito"].ToString(),
                                    referencia = ((reader["referencia"]) is DBNull) ? "" : reader["referencia"].ToString(),
                                    fechaentrega = DateTime.Parse(reader["fechaentrega"].ToString()),
                                    producto = ((reader["producto"]) is DBNull) ? "" : reader["producto"].ToString(),
                                    cantidad = ((reader["cantidad"]) is DBNull) ? 0 : decimal.Parse(reader["cantidad"].ToString())
                                };

                                response.Add(valeDelivery);
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

        public async Task<ResultadoTransaccion<MemoryStream>> GetGenerarValeValeDeliveryReporte3Print(int idvaledelivery)
        {

            ResultadoTransaccion<MemoryStream> vResultadoTransaccion = new ResultadoTransaccion<MemoryStream>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;


            try
            {
                Document doc = new Document();
                doc.SetPageSize(PageSize.A4);
                // points to cm
                doc.SetMargins(20f, 20f, 15f, 15f);
                MemoryStream ms = new MemoryStream();
                PdfWriter write = PdfWriter.GetInstance(doc, ms);
                doc.AddAuthor("Grupo SBA");
                doc.AddTitle("Cliníca San Felipe");
                var pe = new PageEventHelper();
                write.PageEvent = pe;
                // Colocamos la fuente que deseamos que tenga el documento
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                // Titulo
                iTextSharp.text.Font titulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font subTitulo = new iTextSharp.text.Font(helvetica, 12f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegroNegrita = new iTextSharp.text.Font(helvetica, 9f, iTextSharp.text.Font.BOLD, BaseColor.Black);
                iTextSharp.text.Font parrafoNegro = new iTextSharp.text.Font(helvetica, 9f, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                pe.HeaderLeft = " ";
                pe.HeaderFont = parrafoNegroNegrita;
                pe.HeaderRight = " ";
                doc.Open();

                ResultadoTransaccion<BE_ValeDelivery> resultadoTransaccion = await GetListValeDeliveryMedicamentosPorCodigoAtencion(idvaledelivery);

                List<BE_ValeDelivery> response = (List<BE_ValeDelivery>)resultadoTransaccion.dataList;

                if (resultadoTransaccion.ResultadoCodigo == -1)
                {
                    vResultadoTransaccion.IdRegistro = -1;
                    vResultadoTransaccion.ResultadoCodigo = -1;
                    vResultadoTransaccion.ResultadoDescripcion = resultadoTransaccion.ResultadoDescripcion;
                    return vResultadoTransaccion;
                }

                var tbl = new PdfPTable(new float[] { 30f, 40f, 30f }) { WidthPercentage = 100 };

                var c1 = new PdfPCell(new Phrase("Cliníca San Felipe S.A.", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(DateTime.Now.ToString(), parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                tbl = new PdfPTable(new float[] { 10f, 80f, 10f }) { WidthPercentage = 100 };

                var title = string.Format("FORMATO DE ENTREGA FARMACIA A DOMICILIO", titulo);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_LEFT;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase(title, titulo)) { Border = 0 };
                c1.HorizontalAlignment = Element.ALIGN_CENTER;
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                tbl.AddCell(c1);

                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0 };
                c1.VerticalAlignment = Element.ALIGN_MIDDLE;
                c1.HorizontalAlignment = Element.ALIGN_RIGHT;
                tbl.AddCell(c1);
                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                //Cabecera 01

                tbl = new PdfPTable(new float[] { 19f, 30f, 2f, 15f, 33f }) { WidthPercentage = 100 };

                //Linea 1
                c1 = new PdfPCell(new Phrase("Nº Atención", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].codatencion), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                //Linea 2
                c1 = new PdfPCell(new Phrase("Id", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].idvaledelivery.ToString()), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Fecha Registro", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].fecharegistro == null ? "" : (DateTime.Parse(response[0].fecharegistro.ToString())).ToString("dd/MM/yyyy")), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                //Linea 3
                c1 = new PdfPCell(new Phrase("Código Usuario", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].codigousuario), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Nombre Usuario", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombreusuario), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                //Linea 4
                c1 = new PdfPCell(new Phrase("Paciente", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].nombrepaciente), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                c1.Colspan = 4;
                tbl.AddCell(c1);

                //Linea 5
                c1 = new PdfPCell(new Phrase("Teléfono fijo", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].telefono), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Celular", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].celular), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                //Linea 6
                c1 = new PdfPCell(new Phrase("Dirección", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].direccion), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                c1.Colspan = 4;
                tbl.AddCell(c1);

                //Linea 7
                c1 = new PdfPCell(new Phrase("Distrito", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].distrito), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Referencia", parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(string.Format(": {0}", response[0].referencia), parrafoNegro)) { Border = 0, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);

                doc.Add(tbl);

                doc.Add(new Phrase(" "));

                // Generamos el detalle del vale de venta

                tbl = new PdfPTable(new float[] { 15f, 70f, 15f }) { WidthPercentage = 100 };

                c1 = new PdfPCell(new Phrase("Nº", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Producto", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase("Cantidad", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 10, PaddingBottom = 10 };
                tbl.AddCell(c1);

                var cont = 1;

                foreach (BE_ValeDelivery item in response)
                {
                    c1 = new PdfPCell(new Phrase(cont.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.producto.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    c1 = new PdfPCell(new Phrase(item.cantidad.ToString(), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    tbl.AddCell(c1);
                    cont++;
                }

                c1 = new PdfPCell(new Phrase("Fecha entrega", parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                tbl.AddCell(c1);
                c1 = new PdfPCell(new Phrase(response[0].fechaentrega.ToString("dd/MM/yyyy"), parrafoNegro)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingTop = 5, PaddingBottom = 5 };
                c1.Colspan = 2;
                tbl.AddCell(c1);


                doc.Add(tbl);

                write.Close();
                doc.Close();
                ms.Seek(0, SeekOrigin.Begin);
                var file = ms;

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = "Se genero correctamente Vale de Venta";
                vResultadoTransaccion.data = file;
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
