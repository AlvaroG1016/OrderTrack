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


        public async Task<List<string>> GetClavesProductosBD()
        {
            var clavesExistentes = await _context.Productos
                .Where(p => p.Sku != null && p.IdProductoExcel != null)
                .Select(p => $"{p.Sku.Trim().ToUpperInvariant()}-{p.IdProductoExcel}")
                .ToListAsync();

            return clavesExistentes;
        }


        public List<Producto> FiltrarProductosBD(List<Producto> productos, List<string> clavesExistentes)
        {
            var productosNuevos = productos
                .Where(p =>
                    p.Sku != null &&
                    p.IdProductoExcel != null &&
                    !clavesExistentes.Contains($"{p.Sku.Trim().ToUpperInvariant()}-{p.IdProductoExcel}")
                )
                .ToList();

            return productosNuevos;
        }



        public List<Producto> FiltrarProductosMemoria(List<Producto> productos)
        {
            var productosUnicos = productos
                .Where(p => p.Sku != null && p.IdProductoExcel != null)
                .GroupBy(p => $"{p.Sku.Trim().ToUpperInvariant()}-{p.IdProductoExcel}")
                .Select(g => g.First())
                .ToList();

            return productosUnicos;
        }


        public async Task<Dictionary<string, int>> MapeoIdProducto(List<Producto> productosNuevos)
        {
            var clavesProductos = productosNuevos
                .Where(p => p.Sku != null && p.IdProductoExcel != null)
                .Select(p => $"{p.Sku.Trim().ToUpperInvariant()}-{p.IdProductoExcel}")
                .ToList();

            var productoIdMap = await _context.Productos
                .Where(p => p.Sku != null && p.IdProductoExcel != null && clavesProductos.Contains(p.Sku + "-" + p.IdProductoExcel))
                .ToDictionaryAsync(
                    p => $"{p.Sku.Trim().ToUpperInvariant()}-{p.IdProductoExcel}",
                    p => p.IdProducto
                );
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
