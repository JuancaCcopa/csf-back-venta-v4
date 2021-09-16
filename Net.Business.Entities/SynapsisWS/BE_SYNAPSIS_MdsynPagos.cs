using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_SYNAPSIS_MdsynPagos
    {
       
        public long idePagosBot { get; set; }
        public string codTipo { get; set; }
        public int ideMdsynReserva { get; set; }
        public int ideCorrelReserva { get; set; }
        public string codLiquidacion { get; set; }

        public string codVenta { get; set; }
        public decimal cntMontoPago { get; set; }
        public string ideUniqueIdentifier { get; set; }
        public string codRptaSynapsis { get; set; }
        public int usrRegOrdenSynapsis { get; set; }
        public DateTime fecRegOrdenSynapsis { get; set; }
        public string txtJsonOrden { get; set; }
        public string estPagado { get; set; }
        public string nroOperacion { get; set; }
        public string tipTarjeta { get; set; }
        public string numTarjeta { get; set; }
        public string txtJsonRpta { get; set; }
        public DateTime fecRecepcionPago { get; set; }
        //---     EXTENSIONES    ---
        public string nuevoValor { get; set; }
        public string campo { get; set; }
        public string orden { get; set; }

        public BE_SYNAPSIS_OrderApiBot objSynapsisOrderApiBot { get; set; }


        #region Este constructor sirve para utilizar el store: [Sp_MdsynPagos_Update]
        public BE_SYNAPSIS_MdsynPagos() {}

        public BE_SYNAPSIS_MdsynPagos(long pIdePagosBot,
                                        string pCodTipo,
                                        int pIdeMdsynReserva,
                                        int pIdeCorrelReserva,
                                        string pCodLiquidacion,
                                        string pCodVenta,
                                        decimal pCntMontoPago,
                                        string pIdeUniqueIdentifier,
                                        string pCodRptaSynapsis,
                                        int pUsrRegOrdenSynapsis,
                                        string pTxtJsonOrden,
                                        string pEstPagado,
                                        string pNroOperacion,
                                        string pTipTarjeta,
                                        string pNumTarjeta,
                                        string pTxtJsonRpta,
                                        string pOrden)
        {

            idePagosBot = pIdePagosBot;
            ideMdsynReserva = pIdeMdsynReserva;
            codTipo = pCodTipo;
            ideCorrelReserva = pIdeCorrelReserva;
            codLiquidacion = pCodLiquidacion;
            codVenta = pCodVenta;
            cntMontoPago = pCntMontoPago;
            ideUniqueIdentifier = pIdeUniqueIdentifier;
            codRptaSynapsis = pCodRptaSynapsis;
            usrRegOrdenSynapsis = pUsrRegOrdenSynapsis;
            txtJsonOrden = pTxtJsonOrden;
            estPagado = pEstPagado;
            nroOperacion = pNroOperacion;
            tipTarjeta = pTipTarjeta;
            numTarjeta = pNumTarjeta;
            txtJsonRpta = pTxtJsonRpta;
            orden = pOrden;

        }

        #endregion


    }
}
