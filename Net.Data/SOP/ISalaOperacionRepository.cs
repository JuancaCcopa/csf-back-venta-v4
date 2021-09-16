using Net.Business.Entities;
using Net.Connection;
using System.Threading.Tasks;

namespace Net.Data
{
    public interface ISalaOperacionRepository : IRepositoryBase<BE_SalaOperacion>
    {
        Task<ResultadoTransaccion<BE_SalaOperacion>> GetListSalaOperacionPorFiltro(FE_SalaOperacion value);

        Task<ResultadoTransaccion<BE_SalaOperacionRol>> GetListRolSalaOperacionPorAtencion(FE_SalaOperacionAtencion value);
        Task<ResultadoTransaccion<BE_SalaOperacion>> Registrar(BE_SalaOperacionXml value);
        Task<ResultadoTransaccion<BE_SalaOperacion>> ModificarRol(BE_SalaOperacionXml value);
        Task<ResultadoTransaccion<BE_SalaOperacion>> Eliminar(FE_SalaOperacionId value);
        Task<ResultadoTransaccion<BE_SalaOperacion>> Estado(FE_SalaOperacionId value);
    }
}
