using OrderTrack.Models.Domain;

namespace OrderTrack.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<Dictionary<int, List<int>>> DiccionarioIdsPedidos(List<Pedido> pedidos);
    }
}
