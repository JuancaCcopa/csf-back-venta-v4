using Net.Business.Entities;
using Net.Connection;
using System;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IComprobanteRepository : IRepositoryBase<BE_Comprobante>
    {
        Task<ResultadoTransaccion<BE_Comprobante>> GetListaComprobantesPorFiltro(string codcomprobante, DateTime fecinicio, DateTime fecfin, int opcion);
        Task<ResultadoTransaccion<BE_Comprobante>> GetComprobantesElectronico(string codcomprobante_e, string codsistema);
        Task<ResultadoTransaccion<BE_CuadreCaja>> GetListaCuadreCaja(string documento);
        Task<ResultadoTransaccion<BE_Comprobante>> GetComprobanteConsulta(string buscar, int key, int numerolineas, int orden);
        Task<ResultadoTransaccion<string>> ComprobanteDelete(string codcomprobante);
        Task<ResultadoTransaccion<string>> ComprobantesUpdate(string campo, string codigo, string nuevovalor);
    }
}