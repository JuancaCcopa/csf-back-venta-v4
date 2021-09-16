using Net.Business.Entities;
using Net.Connection.Attributes;
using Net.CrossCotting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Net.Business.DTO
{
    [DataContract]
    [Serializable]
    [XmlRoot("SeparacionCuenta")]
    public class DtoSeparacionCuentaRegistrar: EntityBase
    {
        [DataMember, XmlAttribute]
        public string usuario { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListSeparacionCuenta", Type = typeof(List<BE_SeparacionCuenta>))]
        public List<BE_SeparacionCuenta> ListSeparacionCuenta { get; set; }
        public BE_SeparacionCuentaXml RetornaModelo()
        {
            var entiDom = new BE_SeparacionCuentaXml();
            var ser = new Serializador();
            var ms = new MemoryStream();
            ser.SerializarXml(this, ms);
            entiDom.XmlData = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();

            return new BE_SeparacionCuentaXml
            {
                XmlData = entiDom.XmlData,
                RegIdUsuario = this.RegIdUsuario
            };
        }
    }
}
