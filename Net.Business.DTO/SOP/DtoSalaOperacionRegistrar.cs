using Net.Business.Entities;
using Net.CrossCotting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Net.Business.DTO
{
    [DataContract]
    [Serializable]
    [XmlRoot("Cabecera")]
    public class DtoSalaOperacionRegistrar: EntityBase
    {
        [DataMember, XmlAttribute]
        public int codrolsala { get; set; }
        [DataMember, XmlAttribute]
        public string codatencion { get; set; }
        [DataMember, XmlAttribute]
        public string codalmacen { get; set; }
        //[DataMember, XmlAttribute]
        //public DateTime fecharegistro { get; set; }
        //[DataMember, XmlAttribute]
        //public DateTime fechahoraregistro { get; set; }
        //[DataMember, XmlAttribute]
        //public bool flgestado { get; set; }
        //[DataMember, XmlAttribute]
        //public bool flgventa { get; set; }
        //[DataMember, XmlAttribute]
        //public string codventa { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListDetalle", Type = typeof(List<BE_SalaOperacionDetalle>))]
        public List<BE_SalaOperacionDetalle> listaSalaOperacionDetalle { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListDetalleUbicacion", Type = typeof(List<BE_SalaOperacionDetalleUbicacion>))]
        public List<BE_SalaOperacionDetalleUbicacion> listSalaOperacionDetalleUbicacion { get; set; }

        public BE_SalaOperacionXml RetornaModelo()
        {

            var entiDom = new BE_SalaOperacionXml();
            var ser = new Serializador();
            var ms = new MemoryStream();
            ser.SerializarXml(this, ms);
            entiDom.XmlData = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();

            return new BE_SalaOperacionXml
            {
                XmlData = entiDom.XmlData,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
