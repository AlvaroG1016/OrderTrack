using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO.Request;
using OrderTrack.Models.DTO.Response;
using OrderTrack.Services.Interfaces;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

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
        public async Task<List<AgrupacionProductosDto>> ObtenerProductosAgrupados(FiltroReporteProductosDto filtro)
        {
            var pedidos = await _context.DetallePedidos
                .Include(dp => dp.IdProductoNavigation)
                .Include(dp => dp.IdPedidoInternoNavigation)
                .Where(dp =>
                    dp.IdPedidoInternoNavigation.FechaPedido >= DateOnly.FromDateTime(filtro.FechaInicio) &&
                    dp.IdPedidoInternoNavigation.FechaPedido <= DateOnly.FromDateTime(filtro.FechaFin))
                .ToListAsync();

            var agrupado = pedidos
                .GroupBy(dp =>
                {
                    var fecha = dp.IdPedidoInternoNavigation.FechaPedido.ToDateTime(new TimeOnly(0, 0));
                    return filtro.TipoAgrupacion == "mes"
                        ? fecha.ToString("yyyy-MM")
                        : $"{fecha.Year}-W{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                            fecha, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}";
                })
                .Select(g => new AgrupacionProductosDto
                {
                    Agrupacion = g.Key,
                    Productos = g.GroupBy(x => x.IdProductoNavigation.Nombre)
                                 .Select(p => new ProductoVentaDto
                                 {
                                     NombreProducto = p.Key,
                                     TotalVendido = p.Sum(x => x.Cantidad),
                                     TotalIngresos = p.Sum(x => x.PrecioTotal)
                                 })
                                 .OrderByDescending(p => p.TotalVendido)  // Ordenar por más vendidos
                                 .Take(3)                                  // Tomar solo los 3 primeros
                                 .ToList()
                }).ToList();

            return agrupado;
        }


    }
}
