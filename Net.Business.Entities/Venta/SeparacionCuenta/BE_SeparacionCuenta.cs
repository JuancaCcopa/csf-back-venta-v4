using Net.Business.Entities;
using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot("SeparacionCuenta")]
    public class BE_SeparacionCuenta

    {
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 10, ActionType.Everything)]
        public string coddetalle { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codventa { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codalmacen { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int cantidad { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.VarChar, 200, ActionType.Everything)]
        public string nombreproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool manbtchnum { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool binactivat { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListVentasDetalleLote", Type = typeof(List<BE_VentasDetalleLote>))]
        public List<BE_VentasDetalleLote> listVentasDetalleLotes { get; set; }
    }
}
