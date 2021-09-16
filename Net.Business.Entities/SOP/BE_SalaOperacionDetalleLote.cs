using Net.Connection.Attributes;
using System;
using System.Data;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Net.Business.Entities
{
    [DataContract]
    [Serializable]
    [XmlRoot("Lote")]
    public class BE_SalaOperacionDetalleLote
    {
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int idborradorlote { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int idborradordetalle { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codproducto { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.VarChar, 36, ActionType.Everything)]
        public string lote { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechavencimiento { get; set; }
        [DataMember, XmlAttribute]
        [DBParameter(SqlDbType.Decimal, 0, ActionType.Everything)]
        public decimal cantidad { get; set; }
    }
}
