using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;
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
        private readonly IDatatableService _dtService;
        private readonly IMapper _mapper;


        public ProductosService(OrdertrackContext context, IDatatableService datatableService, IMapper mapper)
        {
            _context = context;
            _dtService = datatableService;
            _mapper = mapper;
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

        public async Task<List<AgrupacionProductosDto>> ObtenerTresProductosSeller(FiltroReporteProductosDto filtro)
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
        public async Task<List<UtilidadTotalDTO>> ObtenerTop10ProductosMayorUtilidad()
        {
            var pedidos = await _context.DetallePedidos
                .Include(dp => dp.IdProductoNavigation)
                .Include(dp => dp.IdPedidoInternoNavigation)
                .ToListAsync();

            // Obtener los 10 productos con mayor utilidad
            var productosUtilidadTop10 = pedidos
                .GroupBy(dp => dp.IdProductoNavigation.Nombre)
                .Select(p => new UtilidadTotalDTO
                {
                    NombreProducto = p.Key,
                    // La utilidad ya está calculada en la base de datos, solo la sumamos
                    UtilidadTotal = p.Sum(x => x.Utilidad),
                    // Calculamos el porcentaje de utilidad con respecto al precio del proveedor
                    PorcentajeUtilidad = (p.Sum(x => x.Utilidad) / p.Sum(x => x.PrecioProovedorCantidad)) * 100
                })
                .OrderByDescending(p => p.UtilidadTotal)  // Ordenar por la utilidad total de cada producto
                .Take(10)  // Tomar los 10 productos con mayor utilidad
                .ToList();

            return productosUtilidadTop10;
        }


        public async Task<PaginacionResponseDto<ProductoDTO>> ObtenerDatatableProductos(PaginacionRequestDto filtro)
        {
            var query = _context.Productos.AsQueryable();

            // Aplicar filtros de búsqueda y ordenamiento
            query = _dtService.AplicarFiltrosGenericos(query, filtro, new[] { "Nombre", "Sku", "IdProductoExcel" });

            // Total antes de paginar
            var total = await query.CountAsync();

            // Aplicar paginado
            var data = await query
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync();

            // Mapear solo la página de resultados
            var dataResponse = _mapper.Map<List<ProductoDTO>>(data);

            return new PaginacionResponseDto<ProductoDTO>
            {
                TotalRows = total,
                Data = dataResponse
            };
        }
        public async Task<List<EstadoContadorDto>> ObtenerEstadosPorProductoAsync(int idProducto)
        {
            var existeProducto = await _context.Productos.AnyAsync(p => p.IdProducto == idProducto);
            if (!existeProducto)
            {
                throw new Exception($"El producto con ID {idProducto} no existe.");
            }
            var resultado = await _context.DetallePedidos
                .Where(dp => dp.IdProducto == idProducto)
                .GroupBy(dp => dp.IdPedidoInternoNavigation.Estado)
                .Select(g => new EstadoContadorDto
                {
                    Estado = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            int totalGeneral = resultado.Sum(r => r.Total);

            // Agregar el porcentaje a cada DTO
            foreach (var item in resultado)
            {
                item.Porcentaje = (double)item.Total / totalGeneral * 100;
            }

            return resultado;
        }

    }
}
