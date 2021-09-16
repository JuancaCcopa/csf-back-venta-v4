using Net.Business.Entities;
using Net.Connection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface IValeDeliveryRepository : IRepositoryBase<BE_Receta>
    {
        Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryPorCodAtencion(string codatencion);
        Task<ResultadoTransaccion<BE_ValeDelivery>> RegistrarValeDelivery(BE_ValeDeliveryXml value);
        Task<ResultadoTransaccion<BE_ValeDelivery>> ModificarValeDelivery(BE_ValeDeliveryXml value);
        Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryPorRangoFecha(DateTime fechaInicio, DateTime fechaFinal);
        Task<ResultadoTransaccion<MemoryStream>> GetGenerarValeValeDeliveryReporte1Print(DateTime fechaInicio, DateTime fechaFinal);
        Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryAgrupoEstadoPorRangoFecha(DateTime fechaInicio, DateTime fechaFinal);
        Task<ResultadoTransaccion<MemoryStream>> GetGenerarValeValeDeliveryReporte2Print(DateTime fechaInicio, DateTime fechaFinal);
        Task<ResultadoTransaccion<BE_ValeDelivery>> GetListValeDeliveryMedicamentosPorCodigoAtencion(int idvaledelivery);
        Task<ResultadoTransaccion<MemoryStream>> GetGenerarValeValeDeliveryReporte3Print(int idvaledelivery);
    }
}