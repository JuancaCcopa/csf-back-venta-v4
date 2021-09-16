using Net.Business.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using static Net.Data.SynapsisWSJSON;
//using static Net.Data.SynapsisWS;

namespace Net.Data
{
    public class SynapsisWS
    {

        #region GenerateOrderApiBot
        //BE_SYNAPSIS_MdsynPagos
        public BE_SYNAPSIS_ResponseOrderApiResult GenerateOrderApiBot(B_MdsynPagos pMdsynPagosE,string Synapsis_ApiKey, string Synapsis_SignatureKey, string Synapsis_Ws_Url)
        {

            //BE_SYNAPSIS_ResponseOrderApi responseOrdenApi = new BE_SYNAPSIS_ResponseOrderApi();
            BE_SYNAPSIS_ResponseOrderApiResult oResponseOrderApiResult = new BE_SYNAPSIS_ResponseOrderApiResult();
            //DataTable dt = new DataTable();

            //string rpta = string.Empty, signature = string.Empty;
            //var obj = pMdsynPagosE.objSynapsisOrderApiBot;
            var obj = pMdsynPagosE;

            var order = new BE_SYNAPSIS_Order
            {
                amount = obj.amount,
                number = obj.number,
                customer = new BE_SYNAPSIS_Customer
                {
                    name = obj.cust_name,
                    lastName = obj.cust_lastname,
                    phone = obj.cust_phone,
                    email = obj.cust_email,
                    document = new BE_SYNAPSIS_Document
                    {
                        type = obj.cust_doc_type,
                        number = obj.cust_doc_number
                    },
                    address = new BE_SYNAPSIS_Address
                    {
                        country = obj.cust_adress_country,
                        levels = new string[3] { obj.cust_adress_levels.Substring(0, 2) + "0000", obj.cust_adress_levels.Substring(0, 4) + "00", obj.cust_adress_levels.Substring(0, 4) + "01" },
                        line1 = obj.cust_adress_line1,
                        zip = obj.cust_adress_zip
                    },
                },
                currency = new BE_SYNAPSIS_Currency
                {
                    code = obj.currency_code
                },
                country = new BE_SYNAPSIS_Country
                {
                    code = obj.country_code
                },
                products = new List<BE_SYNAPSIS_Products> {
                    new BE_SYNAPSIS_Products()
                    {
                        name=obj.products_name,
                        quantity=obj.products_quantity,
                        unitAmount=obj.products_unitAmount,
                        amount=obj.products_amount
                    }
                },
                orderType = new BE_SYNAPSIS_OrderType
                {
                    code = obj.ordTyp_code
                },
                targetType = new BE_SYNAPSIS_TargetType
                {
                    code = obj.targTyp_code
                }

            };

            var setting = new BE_SYNAPSIS_Setting
            {
                expiration = new BE_SYNAPSIS_Expiration
                {
                    type = "DATE",
                    date = DateTime.Today.ToString()
                },
                autogenerate = new BE_SYNAPSIS_Autogenerate
                {
                    paymentCode = true
                }
            };

            var objRequestOrderApi = new BE_SYNAPSIS_RequestOrderApi { order = order, settings = setting };

            string jsonBody = JsonConvert.SerializeObject(objRequestOrderApi);
            string valueToSign = Convert.ToString(order.number) + Convert.ToString(order.currency.code) + Convert.ToString(order.amount);
            string signedElement = valueToSign;

            string signature = Criptography.SHA.GenerateSHA512String(Synapsis_ApiKey + valueToSign + Synapsis_SignatureKey).ToLower();
            string urlKey = Synapsis_Ws_Url + "/order/engine/orders/generate";

            Parameters[] arraList = new Parameters[6];
            arraList[0] = new Parameters("Content-Type", "application/json", TipoFormat.Header);
            arraList[1] = new Parameters("identifier", Synapsis_ApiKey, TipoFormat.Header);
            arraList[2] = new Parameters("protocol", "APIKEY", TipoFormat.Header);
            arraList[3] = new Parameters("requireToken", "false", TipoFormat.Header);
            arraList[4] = new Parameters("signedElement", signedElement, TipoFormat.Header);
            arraList[5] = new Parameters("signature", signature, TipoFormat.Header);

            string rpta = MethodPostSignature(urlKey, jsonBody, arraList);

            try
            {

                if (rpta == "")
                {
                    throw new ArgumentException("No se pudo generar la orden de pago, hay problema con el enlace del pago.");
                }
                else
                {
                    var resultApi = JsonConvert.DeserializeObject<BE_SYNAPSIS_ResponseOrderApi>(rpta);
                    oResponseOrderApiResult.jsonBody = jsonBody;
                    oResponseOrderApiResult.responseOrderApi = resultApi;
                }

            }
            catch (Exception ex)
            {
                oResponseOrderApiResult.jsonBody = jsonBody;
                oResponseOrderApiResult.responseOrderApi.message.text = ex.Message;
            }

            return oResponseOrderApiResult;

        }

        #endregion

        #region NotificationOrderPagoBot

        //public Boolean NotificationOrderPagoBot(BE_SYNAPSIS_ResponseNotificationOrderApi.BE_SYNAPSIS_ResponseNotificationOrder objResponseNotificationOrder, string JsonBody)
        //{

        //    Boolean result = false;
        //    //Signature: API Key + Order Number + Currency Code + Order Amount + Payment Unique Identifier + Result Code + Signature Key
        //    objResponseNotificationOrder.signature = Criptography.SHA.GenerateSHA512String("");

        //    return true;
        //}

        #endregion


    }
}
