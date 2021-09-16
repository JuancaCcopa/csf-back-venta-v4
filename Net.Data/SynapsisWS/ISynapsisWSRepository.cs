using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Data
{
    public interface ISynapsisWSRepository
    {
        ResultadoTransaccion<BE_SYNAPSIS_ResponseOrderApiResult> fnGenerarOrdenPagoBot(BE_SYNAPSIS_MdsynPagos objPagosE, int tipoPago, int UsrSistema,string Synapsis_ApiKey,string Synapsis_SignatureKey,string Synapsis_Ws_Url);
        //ResultadoTransaccion<B_MdsynPagos> GetMdsynPagosConsulta(long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden);

        //ResultadoTransaccion<B_MdsynPagos> GetMdsynPagosConsulta(long ide_pagos_bot, int ide_mdsyn_reserva, int ide_correl_reserva, string cod_liquidacion, string cod_venta, int orden, SqlConnection conn);

    }
}
