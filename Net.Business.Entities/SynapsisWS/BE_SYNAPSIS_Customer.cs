using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_Customer
    {
        public string name { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public virtual BE_SYNAPSIS_Document document { get; set; }
        public virtual BE_SYNAPSIS_Address address { get; set; }



    }
}
