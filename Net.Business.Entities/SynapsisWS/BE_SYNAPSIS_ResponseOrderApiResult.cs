using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
   public class BE_SYNAPSIS_ResponseOrderApiResult
    {
        public BE_SYNAPSIS_ResponseOrderApiResult()
        {
            responseOrderApi = new BE_SYNAPSIS_ResponseOrderApi();
        }
        public string jsonBody { get; set; }
        public BE_SYNAPSIS_ResponseOrderApi responseOrderApi { get; set; }
        

    }
}
