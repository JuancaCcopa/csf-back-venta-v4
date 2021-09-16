using Net.Business.Entities;

namespace Net.Business.DTO
{
    public class DtoVentaDevolucionFind
    {
        public string opcion { get; set; }
        public string codatencion { get; set; }
        public string codalmacen { get; set; }

        public BE_VentasDevolucion RetornaModelo()
        {
            return new BE_VentasDevolucion
            {
                opcion = this.opcion,
                codalmacen = this.codalmacen,
                codatencion = this.codatencion
            };
        }
    }
}
