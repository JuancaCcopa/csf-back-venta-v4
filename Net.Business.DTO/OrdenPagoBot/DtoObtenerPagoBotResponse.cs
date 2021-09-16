using Net.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.DTO
{
    public class DtoObtenerPagoBotResponse
    {
        public string est_pagado { get; set; }
        public string flg_pago_usado { get; set; }
        public string nro_operacion { get; set; }
        public string terminal { get; set; }
        public string tip_tarjeta { get; set; }
        public decimal cnt_monto_pago { get; set; }
        public long ide_pagos_bot { get; set; }

        public DtoObtenerPagoBotResponse RetornarObtenerPagoBot(B_MdsynPagos pagos ) {

            return new DtoObtenerPagoBotResponse
            {
                est_pagado=pagos.est_pagado,
                flg_pago_usado = pagos.flg_pago_usado,
                nro_operacion = pagos.nro_operacion,
                terminal = pagos.terminal,
                tip_tarjeta = pagos.tip_tarjeta,
                cnt_monto_pago = pagos.cnt_monto_pago,
                ide_pagos_bot = pagos.ide_pagos_bot,

            };
        }

    }
}
