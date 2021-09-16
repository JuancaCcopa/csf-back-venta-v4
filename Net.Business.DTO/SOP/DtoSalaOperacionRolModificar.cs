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
    public class DtoSalaOperacionRolModificar : EntityBase
    {
        [DataMember, XmlAttribute]
        public int codrolsala { get; set; }
        [DataMember, XmlAttribute]
        public string codatencion { get; set; }
        [DataMember, XmlAttribute]
        public int idborrador { get; set; }

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
