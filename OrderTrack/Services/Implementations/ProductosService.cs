using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class ProductosService : IProductosService
    {
        private readonly OrdertrackContext _context;

        public ProductosService(OrdertrackContext context)
        {
            _context = context;
        }


        public async Task<List<string?>> GetSKUProductosBD()
        {
            var skusExistentes = await _context.Productos
               .Select(p => p.Sku)
               .ToListAsync();

            return skusExistentes;
        }

        public List<Producto> FiltrarProductosBD(List<Producto> productos, List<string> skusExistentes)
        {
            var productosNuevos = productos
               .Where(p => !skusExistentes.Contains(p.Sku))
               .ToList();

            return productosNuevos;
        }

        public List<Producto> FiltrarProductosMemoria(List<Producto> productos)
        {
            var productosUnicos = productos
            .GroupBy(p => p.Sku)
            .Select(g => g.First())
            .ToList();

            return productosUnicos;
        }

        public async Task<Dictionary<string, int>> MapeoIdProducto(List<Producto> productosNuevos)
        {
            var productoIdMap = await _context.Productos
                .Where(p => p.Sku != null && productosNuevos.Select(x => x.Sku).Contains(p.Sku))
                .ToDictionaryAsync(p => p.Sku!, p => p.IdProducto); // El `!` indica que no será null

            return productoIdMap;
        }

        public async Task<List<Producto>> GetAllProductos()
        {
            var result = await _context.Productos
               .ToListAsync();

            return result;
        }
    }
}
