using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.Business.DTO;
using Net.Data;

namespace Net.Business.Services.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "ApiReceta")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ValeDeliveryController : ControllerBase
    {
        private readonly IRepositoryWrapper _repository;
        public ValeDeliveryController(IRepositoryWrapper repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetListValeDeliveryPorCodAtencion([FromQuery] string codatencion)
        {

            var objectGetAll = await _repository.ValeDelivery.GetListValeDeliveryPorCodAtencion(codatencion);

            if (objectGetAll.ResultadoCodigo == -1)
            {
                return BadRequest(objectGetAll);
            }

            return Ok(objectGetAll.dataList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarValeDelivery([FromBody] DtoValeDeliveryRegistrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ValeDelivery.RegistrarValeDelivery(value.RetornaModelo());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }

        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModificarValeDelivery([FromBody] DtoValeDeliveryRegistrar value)
        {
            try
            {
                if (value == null)
                {
                    return BadRequest(ModelState);
                }

                var response = await _repository.ValeDelivery.ModificarValeDelivery(value.RetornaModelo());

                if (response.ResultadoCodigo == -1)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response.dataList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la solicitud : { ex.Message.ToString() }");
            }
        }

        [HttpGet(Name = "GetGenerarValeValeDeliveryReporte1Print")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetGenerarValeValeDeliveryReporte1Print([FromQuery] DateTime fechaInicio, DateTime fechaFinal)
        {
            var objectGetById = await _repository.ValeDelivery.GetGenerarValeValeDeliveryReporte1Print(fechaInicio, fechaFinal);

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf" + fechaInicio.ToString("yyyymmdd") + ".pdf");

            return pdf;
        }

        [HttpGet(Name = "GetGenerarValeValeDeliveryReporte2Print")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GetGenerarValeValeDeliveryReporte2Print([FromQuery] DateTime fechainicio, DateTime fechafin)
        {
            var objectGetById = await _repository.ValeDelivery.GetGenerarValeValeDeliveryReporte2Print(fechainicio, fechafin);

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf" + fechainicio.ToString("yyyymmdd") + ".pdf");

            return pdf;
        }

        [HttpGet(Name = "GetListValeDeliveryPorRangoFecha")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetListValeDeliveryPorRangoFecha([FromQuery] DateTime fechaInicio, DateTime fechafin)
        {
            var response = await _repository.ValeDelivery.GetListValeDeliveryPorRangoFecha(fechaInicio, fechafin);

            if (response.ResultadoCodigo == -1)
            {
                return BadRequest(response);
            }
            else
            {
                return Ok(response.dataList);
            }
        }

        [HttpGet("{idvaledelivery}", Name = "GenerarValeDeliveryPrint")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public async Task<FileContentResult> GenerarValeDeliveryPrint(int idvaledelivery)
        {
            var objectGetById = await _repository.ValeDelivery.GetGenerarValeValeDeliveryReporte3Print(idvaledelivery);

            var pdf = File(objectGetById.data.GetBuffer(), "applicacion/pdf" + idvaledelivery.ToString("yyyymmdd") + ".pdf");

            return pdf;
        }
    }
}
