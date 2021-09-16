namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_ResponseOrderApi
    {
        public BE_SYNAPSIS_ResponseOrderApi()
        {

            message = new BE_SYNAPSIS_message();
            data = new BE_SYNAPSIS_data();

        }

        public bool success { get; set; }
        public BE_SYNAPSIS_message message { get; set; }
        public BE_SYNAPSIS_data data { get; set; }

        public class BE_SYNAPSIS_data
        {
            public BE_SYNAPSIS_data()
            {
                order = new BE_SYNAPSIS_order();
            }
            public BE_SYNAPSIS_order order { get; set; }

        }

        public class BE_SYNAPSIS_order
        {
            public BE_SYNAPSIS_order()
            {
                this.uniqueIdentifier = string.Empty;
            }
            public string uniqueIdentifier { get; set; }
            public int number { get; set; }


        }

    }
}
