using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class SapPaymentCreditCards
    {
        public int CreditCard { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime CardValidUntil { get; set; }
        public string VoucherNum { get; set; }
        public int PaymentMethodCode { get; set; }
        public int NumOfPayments { get; set; }
        public DateTime FirstPaymentDue { get; set; }
        public decimal FirstPaymentSum { get; set; }
        public decimal CreditSum { get; set; }
        public string CreditCur { get; set; }
        public int NumOfCreditPayments { get; set; }
    }
}
