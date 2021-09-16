using Net.Connection.Attributes;
using System.Data;

namespace Net.Business.Entities
{
    public class FE_SalaOperacionAtencion
    {
        [DBParameter(SqlDbType.Char, 8, ActionType.Everything)]
        public string codatencion { get; set; }
    }
}
