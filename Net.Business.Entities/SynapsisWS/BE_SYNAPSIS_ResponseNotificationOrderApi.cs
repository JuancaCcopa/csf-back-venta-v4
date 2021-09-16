using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
   public class BE_SYNAPSIS_ResponseNotificationOrderApi
    {
        public class BE_SYNAPSIS_ResponseNotificationOrder
        {

            public virtual BE_SYNAPSIS_Order Order { get; set; }
            public virtual BE_SYNAPSIS_Payment payment { get; set; }
            public virtual BE_SYNAPSIS_Result result { get; set; }

            // [signature] Firma digital utilizada para asegurar la integridad de la información (SHA-512)
            public string signature { get; set; }
            public Boolean success { get; set; }
            public virtual BE_SYNAPSIS_Message message { get; set; }

        }

        #region Entidad
        public class BE_SYNAPSIS_Order
        {
            public string uniqueIdentifier { get; set; }
            public string number { get; set; }
            public string amount { get; set; }
            public BE_SYNAPSIS_Entity entity { get; set; }
            public BE_SYNAPSIS_Country country { get; set; }
            public BE_SYNAPSIS_Currency currency { get; set; }
            public BE_SYNAPSIS_Products products { get; set; }
            public BE_SYNAPSIS_Customer customer { get; set; }
            public string paymentCode { get; set; }
            public DateTime creation { get; set; }
            public DateTime expiration { get; set; }
            public BE_SYNAPSIS_State state { get; set; }
            public BE_SYNAPSIS_TargetType targetType { get; set; }
            public BE_SYNAPSIS_OrderType orderType { get; set; }
           

        }

        public class BE_SYNAPSIS_Entity
        {
            public string name { get; set; }
            public string identifier { get; set; }
        }

        public class BE_SYNAPSIS_Country
        {
            public string code { get; set; }
        }

        //public class BE_SYNAPSIS_Currency
        //{
        //    public string code { get; set; }
        //    public string name { get; set; }
        //    public string symbol { get; set; }
        //}

        public class BE_SYNAPSIS_Products
        {
            public string name { get; set; }
            public string quantity { get; set; }
            public string unitAmount { get; set; }

        }

        public class BE_SYNAPSIS_State {
            public string code { get; set; }
        }

        public class BE_SYNAPSIS_Payment
        {
            public virtual BE_SYNAPSIS_Card card { get; set; }
            public string uniqueIdentifier { get; set; }
            public virtual BE_SYNAPSIS_Brand brand { get; set; }

        }

        public class BE_SYNAPSIS_Card
        {
            public string bin { get; set; }
            public string lastPan { get; set; }

        }

        public class BE_SYNAPSIS_Brand
        {
            public string code { get; set; }
        }

        public class BE_SYNAPSIS_Result
        {
            public Boolean accepted { get; set; }
            public string code { get; set; }
            public string message { get; set; }
            public BE_SYNAPSIS_Processorresult processorResult { get; set; }
            public string processingTimestamp { get; set; }

        }

        public class BE_SYNAPSIS_Processorresult
        {
            public string code { get; set; }
            public string message { get; set; }
            public string transactionIdentifier { get; set; }
        }

        public class BE_SYNAPSIS_Message
        {
            public string code { get; set; }
            public string text { get; set; }
        }

        #endregion

    }

}


