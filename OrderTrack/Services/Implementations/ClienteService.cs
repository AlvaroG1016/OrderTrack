using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly OrdertrackContext _context;
        public ClienteService(OrdertrackContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> MapeoIdCliente()
        {
            var result = await _context.Clientes
                .ToDictionaryAsync(t => t.Nombre, t => t.IdCliente);

            return result;
        }
        public List<Cliente> FiltrarClientesMemoria(List<Cliente> clientes)
        {
            var tiendasUnicas = clientes
            .GroupBy(t => t.Nombre)
            .Select(g => g.First())
            .ToList();

            return tiendasUnicas;
        }
    }
}
