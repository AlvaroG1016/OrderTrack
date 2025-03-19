using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderTrack.Services.Interfaces;

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
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
