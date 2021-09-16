using Net.Business.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using Net.Connection.ServiceLayer;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using Net.Connection;

namespace Net.Data
{
    public class ClienteRepository : RepositoryBase<BE_Cliente>, IClienteRepository
    {
        private readonly string _cnx;
        private string _aplicacionName;
        private string _metodoName;
        private readonly Regex regex = new Regex(@"<(\w+)>.*");

        const string DB_ESQUEMA = "";
        const string SP_GET_LISTA_CLIENTE_POR_FILTRO = DB_ESQUEMA + "VEN_ListaClientePorFiltro";

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ConnectionServiceLayer _connectServiceLayer;

        public ClienteRepository(IConnectionSQL context, IHttpClientFactory clientFactory, IConfiguration configuration)
            : base(context)
        {
            _cnx = configuration.GetConnectionString("cnnSqlLogistica");
            _configuration = configuration;
            _clientFactory = clientFactory;
            _aplicacionName = this.GetType().Name;
            _connectServiceLayer = new ConnectionServiceLayer(_configuration, _clientFactory);
        }
        public async Task<ResultadoTransaccion<BE_Cliente>> GetListClientePorFiltro(string opcion, string ruc, string nombre)
        {

            ResultadoTransaccion<BE_Cliente> vResultadoTransaccion = new ResultadoTransaccion<BE_Cliente>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                var cadena = "BusinessPartners?$filter = CardType eq 'C'";
                var filter = "";
                var campos = "&$select= CardCode, CardName, FederalTaxID, MailAddress, Phone1, U_SYP_BPTP, U_SYP_BPAP, U_SYP_BPAM, U_SYP_BPNO, U_SYP_BPN2, U_SYP_BPTD";

                if (opcion == "RUC")
                {
                    filter = "&$filter = FederalTaxID eq '" + ruc + "'";
                }

                nombre = nombre == null ? string.Empty : nombre.ToUpper();

                if (opcion == "NOMBRE")
                {
                    filter = "&$filter=contains (CardName , '" + nombre + "' )";
                }

                cadena = cadena + filter + campos;

                List<BE_Cliente> data = await _connectServiceLayer.GetAsync<BE_Cliente>(cadena);

                vResultadoTransaccion.IdRegistro = 0;
                vResultadoTransaccion.ResultadoCodigo = 0;
                vResultadoTransaccion.ResultadoDescripcion = string.Format("Registros Totales {0}", data.Count);
                vResultadoTransaccion.dataList = data;
            }
            catch (Exception ex)
            {
                vResultadoTransaccion.IdRegistro = -1;
                vResultadoTransaccion.ResultadoCodigo = -1;
                vResultadoTransaccion.ResultadoDescripcion = ex.Message.ToString();
            }

            return vResultadoTransaccion;
        }

        public async Task<ResultadoTransaccion<BE_Cliente>> GetListClienteLogisticaPorFiltro(string ruc, string nombre)
        {
            ResultadoTransaccion<BE_Cliente> vResultadoTransaccion = new ResultadoTransaccion<BE_Cliente>();
            _metodoName = regex.Match(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name).Groups[1].Value.ToString();

            vResultadoTransaccion.NombreMetodo = _metodoName;
            vResultadoTransaccion.NombreAplicacion = _aplicacionName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cnx))
                {

                    using (SqlCommand cmd = new SqlCommand(SP_GET_LISTA_CLIENTE_POR_FILTRO, conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ruc", ruc));
                        cmd.Parameters.Add(new SqlParameter("@nombre", nombre));

                        conn.Open();

                        var response = new List<BE_Cliente>();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            response = (List<BE_Cliente>)context.ConvertTo<BE_Cliente>(reader);
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
