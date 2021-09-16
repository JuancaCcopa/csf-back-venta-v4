using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoSalaOperacionCodAtencion
    {
        public string codatencion { get; set; }

        public FE_SalaOperacionAtencion RetornaModelo()
        {
            return new FE_SalaOperacionAtencion
            {
                codatencion = this.codatencion
            };
        }
    }
}
