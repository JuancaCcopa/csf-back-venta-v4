using Net.Connection.Attributes;
using System.Data;

namespace Net.Business.Entities
{
    public class BE_SalaOperacionXml: EntityBase
    {
        [DBParameter(SqlDbType.Int, 0, ActionType.Everything)]
        public int idborrador { get; set; }
        [DBParameter(SqlDbType.Xml, 0, ActionType.Everything)]
        public string XmlData { get; set; }
    }
}
