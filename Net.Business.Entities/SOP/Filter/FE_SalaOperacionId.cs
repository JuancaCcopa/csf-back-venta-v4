using Net.Connection.Attributes;
using System.Data;

namespace Net.Business.Entities
{
    public class FE_SalaOperacionId: EntityBase
    {
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public int idborrador { get; set; }
    }
}
