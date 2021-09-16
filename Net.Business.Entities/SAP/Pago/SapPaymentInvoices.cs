using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class SapPaymentInvoices
    {
        public int DocEntry { get; set; }
        public decimal SumApplied { get; set; }
        public string InvoiceType { get; set; }
    }
}
