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
        private readonly IProductosService _productosService;
        private readonly IPedidoService _pedidoService;
        private readonly ITiendasService _tiendasService;
        private readonly IClienteService _clienteService;
        private readonly OrdertrackContext _context;
        private const int BATCH_SIZE = 10000; // Procesar en bloques de 10,000 registros

        public CargaDatosService(
            IMapper mapper,
            IDataProcessingService dataProcessingService,
            IBulkDataService bulkDataService,
            IExcelService excelService,
            OrdertrackContext context,
            IProductosService productosService,
            IPedidoService pedidoService,
            ITiendasService tiendasService,
            IClienteService clienteService)
        {
            _mapper = mapper;
            _dataProcessingService = dataProcessingService;
            _bulkDataService = bulkDataService;
            _excelService = excelService;
            _context = context;
            _productosService = productosService;
            _pedidoService = pedidoService;
            _tiendasService = tiendasService;
            _clienteService = clienteService;
        }

        public async Task<string> ProcesarCarga(IFormFile file)
        {
            // Leer Excel
            var (pedidosDto, detallesDto, logisticaDto, tiendasDto, novedadesDto, clienteDto, productoDto) = await _excelService.LeerExcel(file);

            //Limpiar datos existentes
            await _dataProcessingService.LimpiarBaseDeDatos();

            //  Mapeo automático con AutoMapper
            var logistica = _mapper.Map<List<Logistica>>(logisticaDto);
            var tiendas = _mapper.Map<List<Tienda>>(tiendasDto);
            var novedades = _mapper.Map<List<Novedade>>(novedadesDto);
            var clientes = _mapper.Map<List<Cliente>>(clienteDto);
            var productos = _mapper.Map<List<Producto>>(productoDto);
            var pedidos = _mapper.Map<List<Pedido>>(pedidosDto);

            var productosNoRepetidos = _productosService.FiltrarProductosMemoria(productos);
            var idProductosExistentes = await _productosService.GetSKUProductosBD();
            var productosNuevos = _productosService.FiltrarProductosBD(productosNoRepetidos, idProductosExistentes);

            //NO BD
            var tiendasUnicas = _tiendasService.FiltrarTiendasMemoria(tiendas);
            var clientesUnicos = _clienteService.FiltrarClientesMemoria(clientes);

            //Insertar en orden, controlando dependencias
            await _bulkDataService.InsertarPorLotesAsync(clientesUnicos, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(tiendasUnicas, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(logistica, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(productosNuevos, BATCH_SIZE);

            var todosProductos = await _productosService.GetAllProductos();
            var idTiendaMap = await _tiendasService.MapeoIdTienda();
            var idClienteMap = await _clienteService.MapeoIdCliente();


            var pedidosMapeados = pedidosDto.Select(pedido => {
                pedido.IdTienda = idTiendaMap.TryGetValue(pedido.NombreTiendaTemp, out int nuevoIdTienda)
                    ? nuevoIdTienda
                    : default(int);

                pedido.IdCliente = idClienteMap.TryGetValue(pedido.NombreClienteTemp, out int nuevoIdCliente)
                    ? nuevoIdCliente
                    : default(int);

                return pedido;
            }).ToList();

            await _bulkDataService.InsertarPorLotesAsync(_mapper.Map<List<Pedido>>(pedidosMapeados), BATCH_SIZE);




            var productoIdMap = await _productosService.MapeoIdProducto(todosProductos);
            var idPedidoMap = await _pedidoService.DiccionarioIdsPedidos(pedidos);
            //var detallesAsignados = new List<DetallePedidoDto>(detallesDto);


            var detallesMapeados = detallesDto.AsParallel().Select((detalle, index) => {
                detalle.IdProducto = productoIdMap.TryGetValue(detalle.SKUTemp, out int nuevoIdProducto)
                    ? nuevoIdProducto
                    : default(int);

                if (idPedidoMap.TryGetValue(detalle.IdPedidoInterno, out List<int> listaIdPedidos))
                {
                    detalle.IdPedidoInterno = listaIdPedidos[index % listaIdPedidos.Count];
                }
                else
                {
                    detalle.IdPedidoInterno = default(int);
                }

                return detalle;
            }).ToList();


            //var detalles = _mapper.Map<List<DetallePedido>>(detallesDto);

            await _bulkDataService.InsertarPorLotesAsync(_mapper.Map<List<DetallePedido>>(detallesMapeados), BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(_mapper.Map<List<Novedade>>(novedadesDto), BATCH_SIZE);


            //  Reactivar índices
            await _dataProcessingService.ReconstruirIndices();

            return "Carga exitosa";
        }
    }
}