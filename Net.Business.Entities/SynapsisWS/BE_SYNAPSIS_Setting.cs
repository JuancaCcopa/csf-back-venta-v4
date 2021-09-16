namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_Setting
    {
        //Public Property language As String = "ES"
        //public string language { get; set; }

        public readonly string language = "ES";
        public BE_SYNAPSIS_Autogenerate autogenerate { get; set; }
        public BE_SYNAPSIS_Expiration expiration { get; set; }



    }


}
