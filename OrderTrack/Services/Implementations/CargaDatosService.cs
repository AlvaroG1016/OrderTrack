using AutoMapper;
using OrderTrack.Models.Domain;

using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{
    public class CargaDatosService
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
            // 1️⃣ 📥 Leer Excel
            var (pedidosDto, detallesDto, logisticaDto, tiendasDto, novedadesDto) = await _excelService.LeerExcel(file);

            // 2️⃣ 🗑 Limpiar datos existentes
            await _dataProcessingService.LimpiarBaseDeDatos();

            // 3️⃣ 🔄 Mapeo automático con AutoMapper
            var pedidos = _mapper.Map<List<Pedido>>(pedidosDto);
            var detalles = _mapper.Map<List<DetallePedido>>(detallesDto);
            var logistica = _mapper.Map<List<Logistica>>(logisticaDto);
            var tiendas = _mapper.Map<List<Tienda>>(tiendasDto);
            var novedades = _mapper.Map<List<Novedade>>(novedadesDto);

            // 4️⃣ 🚀 Insertar en orden, controlando dependencias
            await _bulkDataService.InsertarPorLotesAsync(pedidos, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(detalles, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(logistica, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(tiendas, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(novedades, BATCH_SIZE);

            // 5️⃣ 🔄 Reactivar índices
            await _dataProcessingService.ReconstruirIndices();

            return "Carga exitosa";
        }
    }
}
