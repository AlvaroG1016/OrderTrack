using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class TiendasService : ITiendasService
    {
        private readonly OrdertrackContext _context;

        public TiendasService(OrdertrackContext context)
        {
            _context = context;
        }
        public List<Tienda> FiltrarTiendasMemoria(List<Tienda> tiendas)
        {
            var tiendasUnicas = tiendas
            .GroupBy(t => t.NombreTienda)
            .Select(g => g.First())
            .ToList();

            return tiendasUnicas;
        }
        public async Task<Dictionary<string, int>> MapeoIdTienda()
        {
            var result = await _context.Tiendas
                .ToDictionaryAsync(t => t.NombreTienda, t => t.IdTienda); 

            return result;
        }

    }
}
