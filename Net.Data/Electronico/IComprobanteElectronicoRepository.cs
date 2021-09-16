using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IComprobanteElectronicoRepository : IRepositoryBase<BE_ComprobanteElectronico>
    {
        Task<ResultadoTransaccion<BE_ComprobanteElectronico>> GetListComprobanteElectronicoPorFiltro(string codempresa, string codcomprobante, string codcomprobante_e, string codsistema, string tipocomp_sunat, int orden);
        Task<int> Registrar(string codcomprobante, string tipoCodigo_BarraHash, string tipoOtorgamiento, string Xml);
        //Task<ResultadoTransaccion<BE_ComprobanteElectronico>> ModificarComprobanteElectronico(string campo, string nuevoValor, string XML, string codigo);
        Task<ResultadoTransaccion<string>> EnviarComprobanteElectronica(string tipocomprobante, string comprobante);
    }
}