using OrderTrack.Models.DTO;

namespace OrderTrack.Services.Interfaces
{
    public interface IExcelService
    {
        Task<(List<PedidoDto>, List<DetallePedidoDto>, List<LogisticaDto>, List<TiendaDto>, List<NovedadDto>, List<ClienteDTO>, List<ProductoDTO>)> LeerExcel(IFormFile file);
    }
}