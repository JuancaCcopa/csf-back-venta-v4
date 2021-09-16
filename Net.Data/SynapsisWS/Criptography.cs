using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Net.Data
{
    public static class Criptography
    {
        #region CharactersKey

        private const string CharactersKey = "rpaSPvIvVLlrcmtzPU9/c67Gkj7yL1S5";// 'No se puede alterar la cantidad de caracteres.

        #endregion

        #region EncryptKey

        private const string EncryptKey = "csf3lipe";// 'No se puede alterar la cantidad de caracteres.

        #endregion

        #region EncryptConectionString
        //<summary>
        //Función que devuelve la cadena de conexión o una cadena de texto encriptada.
        //</summary>
        //<param name="pConectionStringDecrypt">Enviar la cadena de conexión desencriptada.</param>
        //<returns></returns>
        public static string EncryptConectionString(string pConectionStringDecrypt)
        {
            var IV = ASCIIEncoding.ASCII.GetBytes(EncryptKey);
            var EncryptionKey = Convert.FromBase64String(CharactersKey);
            var buffer = Encoding.UTF8.GetBytes(pConectionStringDecrypt);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = EncryptionKey;
            des.IV = IV;
            return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));

        }

        #endregion

        #region DecryptConectionString
        //<summary>
        //Función que devuelve la cadena de conexión o una cadena de texto desencriptada.
        //</summary>
        //<param name="pConectionStringEncrypt">Enviar la cadena de conexión encriptada.</param>
        //<returns></returns>
        public static string DecryptConectionString(string pConectionStringEncrypt)
        {
            var IV = ASCIIEncoding.ASCII.GetBytes(EncryptKey);
            var EncryptionKey = Convert.FromBase64String(CharactersKey);
            var buffer = Convert.FromBase64String(pConectionStringEncrypt);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = EncryptionKey;
            des.IV = IV;
            return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }

        #endregion

        #region SHA

        public static class SHA
        {
            public static string GenerateSHA256String(string inputString)
            {
                SHA256 sha256 = SHA256Managed.Create();
                var bytes = Encoding.UTF8.GetBytes(inputString);
                var hash = sha256.ComputeHash(bytes);
                var stringBuilder = new StringBuilder();

                for (int i = 0; i < hash.Length-1; i++)
                {
                    stringBuilder.Append(hash[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }

            public static string GenerateSHA512String(string inputString)
            {
                SHA512 sha512 = SHA512Managed.Create();
                var bytes = Encoding.UTF8.GetBytes(inputString);
                var hash = sha512.ComputeHash(bytes);
                var stringBuilder = new StringBuilder();

                for (int i = 0; i < hash.Length - 1; i++)
                {
                    stringBuilder.Append(hash[i].ToString("X2"));
                }
                return stringBuilder.ToString();
            }

        }

        #endregion


    }
}
