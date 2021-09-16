using System;

namespace Net.Business.Entities
{
    public class BE_ValeDelivery: EntityBase
    {
		public int idvaledelivery { get; set; }
		public int ide_receta { get; set; }
		public string codatencion { get; set; }
		public string nombrepaciente { get; set; }
		public string telefono { get; set; }
		public string celular { get; set; }
		public string direccion { get; set; }
		public string distrito { get; set; }
		public string referencia { get; set; }
		public DateTime fechaentrega { get; set; }
		public DateTime? fecharegistro { get; set; }
		public DateTime? fechaventa { get; set; }
		public string lugarentrega { get; set; }
		public string prioridad_1 { get; set; }
		public string prioridad_2 { get; set; }
		public bool estado { get; set; }
        public string estadovd { get; set; }
        public string codventa { get; set; }
		public int atendido { get; set; }
		public int noDeseaReceta { get; set; }
		public int noSePudoContactar { get; set; }
		public int pendiente { get; set; }
		public int recogidoCSF { get; set; }
		public string codigousuario { get; set; }
		public string nombreusuario { get; set; }
		public string producto { get; set; }
		public decimal cantidad { get; set; }

	}
}
