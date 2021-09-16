using System;

namespace Net.Business.Entities
{
    public class BE_SalaOperacion 
    {
        public int idborrador { get; set; }
        public decimal codrolsala { get; set; }
        public string operacion { get; set; }
        public string codatencion { get; set; }
        public string nombres { get; set; }
        public string codalmacen { get; set; }
        public DateTime fecharegistro { get; set; }
        public DateTime fechahoraregistro { get; set; }
        public bool flgestado { get; set; }
        public bool flgventa { get; set; }
        public string codventa { get; set; }
        public string desalmacen { get; set; }
    }
}
