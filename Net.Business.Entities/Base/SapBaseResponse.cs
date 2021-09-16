using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class SapBaseResponse<T>
    {
        /// <summary>
        /// ID de un nuevo registro o Id de registro actualizar
        /// </summary>
        public int DocEntry { get; set; }
        public string Mensaje { get; set; }
        public T data { get; set; }
    }
}
