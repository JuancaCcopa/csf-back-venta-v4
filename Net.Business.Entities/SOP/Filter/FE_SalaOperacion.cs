using Net.Connection.Attributes;
using System;
using System.Data;

namespace Net.Business.Entities
{
    public class FE_SalaOperacion
    {
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechainicio { get; set; }
        [DBParameter(SqlDbType.DateTime, 0, ActionType.Everything)]
        public DateTime fechafin { get; set; }
    }
}
