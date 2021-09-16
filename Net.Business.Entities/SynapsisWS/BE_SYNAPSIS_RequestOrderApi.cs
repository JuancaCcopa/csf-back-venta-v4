namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_RequestOrderApi
    {
        public BE_SYNAPSIS_RequestOrderApi()
        {
            settings = new BE_SYNAPSIS_Setting();
            order = new BE_SYNAPSIS_Order();

        }
        public virtual BE_SYNAPSIS_Order order { get; set; }
        public virtual BE_SYNAPSIS_Setting settings { get; set; }


    }

}
