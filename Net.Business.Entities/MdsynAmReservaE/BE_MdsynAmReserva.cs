using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Business.Entities
{
    public class BE_MdsynAmReserva
    {
        public long ideReserva { get; set; }
        public int ideCorrelReserva { get; set; }
        public string codSede { get; set; }
        public string codPaciente { get; set; }
        public string rutPaciente { get; set; }
        public string codMedico { get; set; }
        public string codProfMedico { get; set; }
        public string codEspecialidad { get; set; }
        public DateTime fecCita { get; set; }
        public string usrRegistroPlataforma { get; set; }
        public DateTime fecRegistro { get; set; }
        public string estConsultaMedica { get; set; }
        public decimal cntMontopago { get; set; }
        public string codTipoPago { get; set; }
        public string flgReservaAnulada { get; set; }
        public DateTime fecReservaAnulada { get; set; }
        public int usrReservaAnulada { get; set; }
        public int usrReprograma { get; set; }
        public int usrRecepciona { get; set; }
        public string flgReprogramar { get; set; }

        //---     EXTENSIONES    ---

        public string nuevoValor { get; set; }
        public string campo { get; set; }
        public string orden { get; set; }
        public string fecInicio { get; set; }
        public string fecFin { get; set; }
        public string nombresPaciente { get; set; }
        public string nombresMedico { get; set; }
        public string telefono { get; set; }
        public string telefono2 { get; set; }
        public string dscSede { get; set; }
        public string dscEspecialidad { get; set; }
        public string dscTipoConsultaMedica { get; set; }
        public int idePagosBot { get; set; }
        public string dscTipoPago { get; set; }
        public string codTipoConsultaMedica { get; set; }
        public string codAtencion { get; set; }
        public string flgUpdateFechas { get; set; }
        public string estPago { get; set; }

        public int ideCorrelReservaBack { get; set; }
        public int ideCorrelReservaNew { get; set; }
        public string codPresotor { get; set; }

        public string codSedeBack { get; set; }
        public string codSedeNew { get; set; }
        public string email { get; set; }

        public string fecAnulacion { get; set; }
        public string moneda { get; set; }
        public string monto { get; set; }
        public string nroOperacion { get; set; }

        #region Sirve para usar en el método Sp_MdsynAmReserva_Update
        public BE_MdsynAmReserva(long pIdeReserva,
                                    int pIdeCorrelReserva,
                                    string pCodSede,
                                    string pCodPaciente,
                                    string pRutPaciente,
                                    string pCodMedico,
                                    string pCodProfMedico,
                                    string pCodEspecialidad,
                                    DateTime pFecCita,
                                    string pUsrRegistroPlataforma,
                                    decimal pCntMontoPago,
                                    string pCodTipoPago,
                                    string pOrden,
                                    int pUsrReservaAnulada,
                                    string pFlgReservaAnulada)
        {

            ideReserva = pIdeReserva;
            ideCorrelReserva = pIdeCorrelReserva;
            codSede = pCodSede;
            codPaciente = pCodPaciente;
            rutPaciente = pRutPaciente;
            codMedico = pCodMedico;
            codProfMedico = pCodProfMedico;
            codEspecialidad = pCodEspecialidad;
            fecCita = pFecCita;
            usrRegistroPlataforma = pUsrRegistroPlataforma;
            cntMontopago = pCntMontoPago;
            codTipoPago = pCodTipoPago;
            orden = pOrden;
            usrReservaAnulada = pUsrReservaAnulada;
            flgReservaAnulada = pFlgReservaAnulada;

        }

        #endregion

    }
}
