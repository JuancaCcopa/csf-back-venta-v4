using System;

namespace Net.Business.Entities
{
    public class SapPaymentChecks
    {
        public DateTime DueDate { get; set; }
        public int CheckNumber { get; set; }
        public string BankCode { get; set; }
        public decimal CheckSum { get; set; }
        public string Currency { get; set; }
        public string CountryCode { get; set; }
    }
}
