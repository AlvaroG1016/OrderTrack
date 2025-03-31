using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class PedidoService :IPedidoService
    {
        private readonly OrdertrackContext _context;

        public PedidoService(OrdertrackContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<int, List<int>>> DiccionarioIdsPedidos(List<Pedido>pedidos)
        {
            var result = await _context.Pedidos
             .OrderBy(p => p.IdPedidoInterno) // Asegurar orden
             .GroupBy(p => p.IdPedido)
             .ToDictionaryAsync(g => g.Key, g => g.Select(p => p.IdPedidoInterno).ToList());

            return result;
        }
    }
}
