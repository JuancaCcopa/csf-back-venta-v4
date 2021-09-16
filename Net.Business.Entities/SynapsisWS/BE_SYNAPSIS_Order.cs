using System.Collections.Generic;

namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_Order
    {
        public decimal amount { get; set; }
        public long number { get; set; }
        public virtual BE_SYNAPSIS_Customer customer { get; set; }
        public virtual BE_SYNAPSIS_Currency currency { get; set; }
        public virtual BE_SYNAPSIS_Country country { get; set; }
        public virtual IList<BE_SYNAPSIS_Products> products { get; set; }
        public virtual BE_SYNAPSIS_OrderType orderType { get; set; }
        public virtual BE_SYNAPSIS_TargetType targetType { get; set; }

    }
}
