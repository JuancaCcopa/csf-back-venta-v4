using Net.Connection.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot("Detalle")]

    public class BE_SalaOperacionDetalle
    {
        //[DataMember, XmlAttribute]
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int idborradordetalle { get; set; }
        //[DataMember, XmlAttribute]
        //[DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        //public int idborrador { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.VarChar, 100, ActionType.Everything)]
        public string nombreproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal cantidad { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool manbtchnum { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool binactivat { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool flgbtchnum { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Bit, 0, ActionType.Everything)]
        public bool flgbin { get; set; }
        [DataMember]
        [XmlElement(ElementName = "ListSalaOperacionDetalleLote", Type = typeof(List<BE_SalaOperacionDetalleLote>))]
        public List<BE_SalaOperacionDetalleLote> listaSalaOperacionDetalleLote { get; set; }
    }
}
