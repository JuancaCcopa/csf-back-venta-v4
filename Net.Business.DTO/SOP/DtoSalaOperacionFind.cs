using Net.Business.Entities;
using System;

namespace Net.Business.DTO
{
    public class DtoSalaOperacionFind
    {
        public DateTime fechainicio { get; set; }
        public DateTime fechafin { get; set; }

        public FE_SalaOperacion RetornaModelo()
        {
            return new FE_SalaOperacion
            {
                fechainicio = this.fechainicio,
                fechafin = this.fechafin
            };
        }
    }
}
