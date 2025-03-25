using AutoMapper;
using OrderTrack.Models.Domain;

using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class CargaDatosService : ICargaDatosService
    {
        private readonly IMapper _mapper;
        private readonly IDataProcessingService _dataProcessingService;
        private readonly IBulkDataService _bulkDataService;
        private readonly IExcelService _excelService;
        private const int BATCH_SIZE = 10000; // Procesar en bloques de 10,000 registros

        public CargaDatosService(
            IMapper mapper,
            IDataProcessingService dataProcessingService,
            IBulkDataService bulkDataService,
            IExcelService excelService)
        {
            _mapper = mapper;
            _dataProcessingService = dataProcessingService;
            _bulkDataService = bulkDataService;
            _excelService = excelService;
        }

        public async Task<string> ProcesarCarga(IFormFile file)
        {
            // Leer Excel
            var (pedidosDto, detallesDto, logisticaDto, tiendasDto, novedadesDto, clienteDto, productoDto) = await _excelService.LeerExcel(file);

            //Limpiar datos existentes
            await _dataProcessingService.LimpiarBaseDeDatos();

            //  Mapeo automático con AutoMapper
            var pedidos = _mapper.Map<List<Pedido>>(pedidosDto);
            var detalles = _mapper.Map<List<DetallePedido>>(detallesDto);
            var logistica = _mapper.Map<List<Logistica>>(logisticaDto);
            var tiendas = _mapper.Map<List<Tienda>>(tiendasDto);
            var novedades = _mapper.Map<List<Novedade>>(novedadesDto);
            var clientes = _mapper.Map<List<Cliente>>(clienteDto);
            var productos = _mapper.Map<List<Producto>>(productoDto);

            //Insertar en orden, controlando dependencias
            await _bulkDataService.InsertarPorLotesAsync(clientes, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(tiendas, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(pedidos, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(logistica, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(productos, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(detalles, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(novedades, BATCH_SIZE);

            //  Reactivar índices
            await _dataProcessingService.ReconstruirIndices();

            return "Carga exitosa";
        }
    }
}
