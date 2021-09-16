using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_OrderApiBot
    {
        public double amount { get; set; }
        public int number { get; set; }
        public string cust_name { get; set; }
        public string cust_lastname { get; set; }
        public string cust_phone { get; set; }
        public string cust_email { get; set; }
        public string cust_doc_type { get; set; }
        public string cust_doc_number { get; set; }
        public string cust_adress_country { get; set; }
        public string cust_adress_levels { get; set; }
        public string cust_adress_line1 { get; set; }
        public string cust_adress_zip { get; set; }
        public string currency_code { get; set; }
        public string country_code { get; set; }
        public string products_name { get; set; }
        public int products_quantity { get; set; }
        public double products_unitAmount { get; set; }
        public double products_amount { get; set; }
        public string ordTyp_code { get; set; }
        public string targTyp_code { get; set; }
        public string header_ApiKey { get; set; }
        public string header_SecretKey { get; set; }
        public string setting_expiration_date { get; set; }

       

    }
}
