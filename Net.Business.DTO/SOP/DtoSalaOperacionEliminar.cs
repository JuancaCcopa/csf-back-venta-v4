using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoSalaOperacionEliminar: EntityBase
    {
        public int idborrador { get; set; }

        public FE_SalaOperacionId RetornaModelo()
        {
            return new FE_SalaOperacionId
            {
                idborrador = this.idborrador,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
