using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;
using OrderTrack.Services.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

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
            ITiendasService tiendasService)
        {
            _mapper = mapper;
            _dataProcessingService = dataProcessingService;
            _bulkDataService = bulkDataService;
            _excelService = excelService;
            _context = context;
            _productosService = productosService;
            _pedidoService = pedidoService;
            _tiendasService = tiendasService;
        }

        public async Task<string> ProcesarCarga(IFormFile file)
        {
            // Leer Excel
            var (pedidosDto, detallesDto, logisticaDto, tiendasDto, novedadesDto, clienteDto, productoDto) = await _excelService.LeerExcel(file);

            // Limpiar datos existentes
            await _dataProcessingService.LimpiarBaseDeDatos();

            // Mapeo automático con AutoMapper
            var logistica = _mapper.Map<List<Logistica>>(logisticaDto);
            var tiendas = _mapper.Map<List<Tienda>>(tiendasDto);
            var novedades = _mapper.Map<List<Novedade>>(novedadesDto);
            var clientes = _mapper.Map<List<Cliente>>(clienteDto);
            var productos = _mapper.Map<List<Producto>>(productoDto);

            // Filtrar datos
            var productosNoRepetidos = _productosService.FiltrarProductosMemoria(productos);
            var tiendasUnicas = _tiendasService.FiltrarTiendasMemoria(tiendas);
            var idProductosExistentes = await _productosService.GetSKUProductosBD();
            var productosNuevos = _productosService.FiltrarProductosBD(productosNoRepetidos, idProductosExistentes);

            // Insertar en orden, controlando dependencias con inserciones por lotes
            await _bulkDataService.InsertarPorLotesAsync(clientes, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(tiendasUnicas, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(logistica, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(productosNuevos, BATCH_SIZE);

            // Mapear detalles y pedidos para tener las entidades completas
            var detalles = _mapper.Map<List<DetallePedido>>(detallesDto);
            var pedidos = _mapper.Map<List<Pedido>>(pedidosDto);

            // Insertar entidades principales
            await _bulkDataService.InsertarPorLotesAsync(novedades, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(pedidos, BATCH_SIZE);
            await _bulkDataService.InsertarPorLotesAsync(detalles, BATCH_SIZE);

            // Ahora optimizamos el manejo de relaciones con SQL directo
            await ActualizarRelacionesConSQL(detallesDto, pedidosDto);

            // Reactivar índices
            await _dataProcessingService.ReconstruirIndices();

            return "Carga exitosa";
        }
        //TODO: NO SE ENVIA BIEN PORQUE EL ID QUE SE USA ES EL DEL DTO, ES DECIR, 0 O NULL, SE DEBE ENVIAR EL DE BD 
        private async Task ActualizarRelacionesConSQL(List<DetallePedidoDto> detallesDto, List<PedidoDto> pedidosDto)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                // Crear tablas temporales para mapeo
                await connection.ExecuteAsync(@"
                    IF OBJECT_ID('tempdb..#TempDetallesMapping') IS NOT NULL DROP TABLE #TempDetallesMapping;
                    IF OBJECT_ID('tempdb..#TempPedidosMapping') IS NOT NULL DROP TABLE #TempPedidosMapping;

                    CREATE TABLE #TempDetallesMapping (
                        IdDetalle INT,
                        SKUTemp NVARCHAR(100),
                        IdPedidoInterno NVARCHAR(100)
                    );
                    
                    CREATE TABLE #TempPedidosMapping (
                        IdPedido INT,
                        IdPedidoInterno NVARCHAR(100),
                        NombreTiendaTemp NVARCHAR(100)
                    );
                ");

                // Cargar datos de mapeo para detalles
                var detallesTable = new DataTable();
                detallesTable.Columns.Add("IdDetalle", typeof(int));
                detallesTable.Columns.Add("SKUTemp", typeof(string));
                detallesTable.Columns.Add("IdPedidoInterno", typeof(string));

                foreach (var detalle in detallesDto)
                {
                    detallesTable.Rows.Add(detalle.IdDetalle, detalle.SKUTemp, detalle.IdPedidoInterno);
                }

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "#TempDetallesMapping";
                    bulkCopy.ColumnMappings.Add("IdDetalle", "IdDetalle");
                    bulkCopy.ColumnMappings.Add("SKUTemp", "SKUTemp");
                    bulkCopy.ColumnMappings.Add("IdPedidoInterno", "IdPedidoInterno");
                    await bulkCopy.WriteToServerAsync(detallesTable);
                }

                // Cargar datos de mapeo para pedidos
                var pedidosTable = new DataTable();
                pedidosTable.Columns.Add("IdPedido", typeof(int));
                pedidosTable.Columns.Add("IdPedidoInterno", typeof(string));
                pedidosTable.Columns.Add("NombreTiendaTemp", typeof(string));

                foreach (var pedido in pedidosDto)
                {
                    pedidosTable.Rows.Add(pedido.IdPedido, pedido.IdPedidoInterno, pedido.NombreTiendaTemp);
                }

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "#TempPedidosMapping";
                    bulkCopy.ColumnMappings.Add("IdPedido", "IdPedido");
                    bulkCopy.ColumnMappings.Add("IdPedidoInterno", "IdPedidoInterno");
                    bulkCopy.ColumnMappings.Add("NombreTiendaTemp", "NombreTiendaTemp");
                    await bulkCopy.WriteToServerAsync(pedidosTable);
                }

                // Ejecutar actualizaciones en batch para todas las relaciones de una vez
                await connection.ExecuteAsync(@"
                    -- Actualizar IdProducto en DetallePedido basado en SKUTemp
                    UPDATE dp
                    SET IdProducto = p.IdProducto
                    FROM DetallePedido dp
                    JOIN #TempDetallesMapping tdm ON dp.IdDetalle = tdm.IdDetalle
                    JOIN Productos p ON tdm.SKUTemp = p.SKU;

                    -- Actualizar IdPedido en DetallePedido basado en IdPedidoInterno
                    UPDATE dp
                    SET IdPedidoInterno = p.IdPedidoInterno
                    FROM DetallePedido dp
                    JOIN #TempDetallesMapping tdm ON dp.IdDetalle = tdm.IdDetalle
                    JOIN Pedidos p ON tdm.IdPedidoInterno = p.IdPedido;

                    -- Actualizar IdTienda en Pedido basado en NombreTiendaTemp
                    UPDATE p
                    SET IdTienda = t.IdTienda
                    FROM Pedidos p
                    JOIN #TempPedidosMapping tpm ON p.IdPedido = tpm.IdPedido
                    JOIN Tiendas t ON tpm.NombreTiendaTemp = t.NombreTienda;

                    -- Limpiar tablas temporales
                    DROP TABLE #TempDetallesMapping;
                    DROP TABLE #TempPedidosMapping;
                ");
            }
        }
    }
}