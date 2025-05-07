using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderTrack.Models.DTO.Request;
using OrderTrack.Services.Implementations;
using OrderTrack.Services.Interfaces;
using OrderTrack.Utilities;

namespace OrderTrack.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductosService _service;

        public ProductoController(IProductosService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerProductosAgrupados([FromBody] FiltroReporteProductosDto filtro)
        {
            try
            {

                var resultado = await _service.ObtenerTresProductosSeller(filtro);
                return Ok(ResponseBuilder.BuildSuccessResponse(resultado));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseBuilder.BuildErrorResponse(ex.Message));

            }
        }
        [HttpPost]
        public async Task<IActionResult> GetDatatableProductos([FromBody] PaginacionRequestDto filtro)
        {
            try
            {

                var resultado = await _service.ObtenerDatatableProductos(filtro);
                return Ok(ResponseBuilder.BuildSuccessResponse(resultado));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseBuilder.BuildErrorResponse(ex.Message));

            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductosUtilidad()
        {
            try
            {

                var resultado = await _service.ObtenerTop10ProductosMayorUtilidad();
                return Ok(ResponseBuilder.BuildSuccessResponse(resultado));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseBuilder.BuildErrorResponse(ex.Message));

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEstadoProductos(int idProducto)
        {
            try
            {

                var resultado = await _service.ObtenerEstadosPorProductoAsync(idProducto);
                return Ok(ResponseBuilder.BuildSuccessResponse(resultado));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseBuilder.BuildErrorResponse(ex.Message));

            }
        }
    }
}
