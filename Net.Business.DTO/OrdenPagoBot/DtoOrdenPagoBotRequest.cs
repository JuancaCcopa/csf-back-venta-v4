using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoOrdenPagoBotRequest
    {
        public long idePagosBot { get; set; }
        public string codtipoPago { get; set; }
        public int idMdsynReserva { get; set; }
        public int idCorrelReservaMedisyn { get; set; }
        public string codLiquidacion { get; set; }
        public string codVenta { get; set; }
        public decimal montoPagar { get; set; }
        public int idUsuario { get; set; }

        public BE_SYNAPSIS_MdsynPagos RetornaMdsynPagos()
        {
            var obj = new BE_SYNAPSIS_MdsynPagos
            {
                idePagosBot = idePagosBot,
                codTipo = codtipoPago,
                ideMdsynReserva = idMdsynReserva,
                ideCorrelReserva = idCorrelReservaMedisyn,
                codLiquidacion = codLiquidacion,
                codVenta = codVenta,
                cntMontoPago = montoPagar,
            };

            return obj;

        }
    }

}
