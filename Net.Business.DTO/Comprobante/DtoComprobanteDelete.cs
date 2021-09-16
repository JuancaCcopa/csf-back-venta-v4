using Net.Business.Entities;


namespace Net.Business.DTO
{
    public class DtoComprobanteDelete
    {

        public string codComprobante { get; set; }
        public int idUsuario { get; set; }
       
        public BE_Comprobante RetornaComprobanteDelete() {

            var obj = new BE_Comprobante();
            obj.codcomprobante = this.codComprobante;
            obj.idusuario = this.idUsuario;
            return obj;
           
        }

    }
}
