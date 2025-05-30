﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderTrack.Services.Interfaces;
using OrderTrack.Utilities;

namespace OrderTrack.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CargueDatosController : ControllerBase
    {
        private readonly ICargaDatosService _cargaDatosService;

        public CargueDatosController(ICargaDatosService cargaDatosService)
        {
            _cargaDatosService = cargaDatosService;
        }

        [HttpPost()]
        public async Task<IActionResult> CargarExcel(IFormFile file)
        {
            try
            {
                var resultado = await _cargaDatosService.ProcesarCarga(file);
                return Ok(ResponseBuilder.BuildSuccessResponse(resultado));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseBuilder.BuildErrorResponse(ex.Message));
            }
        }
    }
}
