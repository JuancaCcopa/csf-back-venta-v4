using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace Net.Data
{
    public class SynapsisWSJSON
    {
        //<summary>
        //Consumir el servicio web.
        //</summary>
        //<param name="Url">Url del método</param>
        //<param name="StringJsonBody">Cuerpo Json del string, en caso no desee enviar utilizar el formato JSON enviar vacio.</param>
        //<param name="parametros">Parametros del método a consumir, puede ser parametros del Header y Body</param>
        //<returns></returns>
        #region MethodPostSignature

        public static string MethodPostSignature(string Url, string StringJsonBody, Parameters[] parametros)
        {
            var Client = new RestClient(Url);
            var Request = new RestRequest(Method.POST);

            //Cargar los request del header
            for (int i = 0; i < parametros.LongLength; i++)
            {
                if (parametros[i].Tipo == TipoFormat.Header)
                {
                    Request.AddHeader(parametros[i].Key, parametros[i].Value);
                }
            }

            //Cargar los request del body.
            for (int i = 0; i < parametros.LongLength; i++)
            {
                if (parametros[i].Tipo == TipoFormat.Body)
                {
                    Request.AddHeader(parametros[i].Key, parametros[i].Value);
                }
            }

            //'StringJsonBody = stringBody()
            if (Url.IndexOf("https") >= 0)
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            }

            Request.AddParameter("application/json", StringJsonBody, ParameterType.RequestBody);
            IRestResponse response = Client.Execute(Request);

            return response.Content;

        }

        #endregion



        public class Parameters
        {
            public Parameters() { }
            public Parameters(string pKey, string pValue)
            {
                Key = pKey;
                Value = pValue;
            }
            public Parameters(string pKey, string pValue, TipoFormat pTipo)
            {
                Key = pKey;
                Value = pValue;
                Tipo = pTipo;
            }

            public string Key { get; set; }
            public string Value { get; set; }
            public TipoFormat Tipo { get; set; }

        }

        public enum TipoFormat
        {
            Body = 1,
            Header = 2
        }

    }
}
