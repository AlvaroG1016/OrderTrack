using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;
using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class CargaDatosService : ICargaDatosService
    {
        private readonly IMapper _mapper;
        private readonly IDataProcessingService _dataProcessingService;
        private readonly IBulkDataService _bulkDataService;
        private readonly IExcelService _excelService;
        private readonly OrdertrackContext _context;
        private const int BATCH_SIZE = 10000; // Procesar en bloques de 10,000 registros

        public CargaDatosService(
            IMapper mapper,
            IDataProcessingService dataProcessingService,
            IBulkDataService bulkDataService,
            IExcelService excelService,
            OrdertrackContext context)
        {
            _mapper = mapper;
            _dataProcessingService = dataProcessingService;
            _bulkDataService = bulkDataService;
            _excelService = excelService;
            _context = context;
        }

        public async Task<string> ProcesarCarga(IFormFile file)
        {
            // Leer Excel
            var (pedidosDto, detallesDto, logisticaDto, tiendasDto, novedadesDto, clienteDto, productoDto) = await _excelService.LeerExcel(file);

            //Limpiar datos existentes
            await _dataProcessingService.LimpiarBaseDeDatos();

            //  Mapeo automático con AutoMapper
            var pedidos = _mapper.Map<List<Pedido>>(pedidosDto);
            var logistica = _mapper.Map<List<Logistica>>(logisticaDto);
            var tiendas = _mapper.Map<List<Tienda>>(tiendasDto);
            var novedades = _mapper.Map<List<Novedade>>(novedadesDto);
            var clientes = _mapper.Map<List<Cliente>>(clienteDto);
            var productos = _mapper.Map<List<Producto>>(productoDto);

            var productosNoRepetidos = _bulkDataService.FiltrarProductosMemoria(productos);
            var idProductosExistentes  = await _bulkDataService.GetProductosBD();
            var productosNuevos = _bulkDataService.FiltrarProductosBD(productosNoRepetidos, idProductosExistentes);

            var detallesAsignados = new List<DetallePedidoDto>(detallesDto);


            //Insertar en orden, controlando dependencias
            await _bulkDataService.InsertarPorLotesAsync(clientes, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(tiendas, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(pedidos, BATCH_SIZE);

            var idPedidoMap = await _context.Pedidos
             .Where(p => pedidos.Select(x => x.IdPedido).Contains(p.IdPedido))
             .OrderBy(p => p.IdPedidoInterno) // Asegurar orden
             .GroupBy(p => p.IdPedido)
             .ToDictionaryAsync(g => g.Key, g => g.Select(p => p.IdPedidoInterno).ToList());


            await _bulkDataService.InsertarPorLotesAsync(logistica, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(productosNuevos, BATCH_SIZE);

            var productoIdMap = await _bulkDataService.MapeoIdProducto(productosNuevos);

            foreach (var detalle in detallesDto)
            {
                if (productoIdMap.TryGetValue(detalle.SKUTemp, out int nuevoIdProducto))
                {
                    detalle.IdProducto = nuevoIdProducto; // Asignar el nuevo ID generado
                }

                if (idPedidoMap.TryGetValue(detalle.IdPedidoInterno, out List<int> listaIdPedidos))
                {
                    int index = detallesAsignados.Count % listaIdPedidos.Count;
                    detalle.IdPedidoInterno = listaIdPedidos[index];
                }

            }

            var detalles = _mapper.Map<List<DetallePedido>>(detallesDto);

            await _bulkDataService.InsertarPorLotesAsync(detalles, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(novedades, BATCH_SIZE);

            //  Reactivar índices
            await _dataProcessingService.ReconstruirIndices();

            return "Carga exitosa";
        }
    }
}
