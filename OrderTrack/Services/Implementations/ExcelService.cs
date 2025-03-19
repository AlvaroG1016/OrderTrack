using ExcelDataReader;
using OrderTrack.Models.DTO;
using OrderTrack.Services.Interfaces;
using System.Data;

namespace OrderTrack.Services.Implementations
{
    public class ExcelService : IExcelService
    {
        public async Task<(List<PedidoDto>, List<DetallePedidoDto>, List<LogisticaDto>, List<TiendaDto>, List<NovedadDto>)> LeerExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("El archivo es inválido.");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet();
            var table = dataSet.Tables[0];

            var pedidos = new List<PedidoDto>();
            var detalles = new List<DetallePedidoDto>();
            var logistica = new List<LogisticaDto>();
            var tiendas = new List<TiendaDto>();
            var novedades = new List<NovedadDto>();

            foreach (DataRow row in table.Rows)
            {
                // Convertir valores con TryParse para evitar errores de conversión
                int.TryParse(row[0]?.ToString(), out int pedidoId);
                int.TryParse(row[1]?.ToString(), out int clienteId);
                DateOnly.TryParse(row[2]?.ToString(), out DateOnly fechaPedido);
                int.TryParse(row[3]?.ToString(), out int productoId);
                int.TryParse(row[4]?.ToString(), out int cantidad);
                decimal.TryParse(row[5]?.ToString(), out decimal precioUnitario);
                string estadoEnvio = row[6]?.ToString() ?? "Desconocido";
                DateOnly.TryParse(row[7]?.ToString(), out DateOnly fechaEntrega);
                int.TryParse(row[8]?.ToString(), out int tiendaId);
                string nombreTienda = row[9]?.ToString() ?? "No especificado";
                string ubicacionTienda = row[10]?.ToString() ?? "No especificada";
                int.TryParse(row[11]?.ToString(), out int novedadId);
                string descripcion = row[12]?.ToString() ?? "Sin descripción";
                DateTime.TryParse(row[13]?.ToString(), out DateTime fechaNovedad);

                // Agregar a las listas
                pedidos.Add(new PedidoDto { IdPedido = pedidoId, IdCliente = clienteId, FechaPedido = fechaPedido });
                detalles.Add(new DetallePedidoDto { IdPedidoInterno = pedidoId, IdProducto = productoId, Cantidad = cantidad, PrecioUnitario = precioUnitario });
                logistica.Add(new LogisticaDto { IdPedidoInterno = pedidoId, FechaGuiaGenerada= fechaEntrega });
                tiendas.Add(new TiendaDto { IdTienda = tiendaId, Tienda1 = nombreTienda});
                novedades.Add(new NovedadDto { IdNovedad = novedadId, IdPedido = pedidoId});
            }


            return (pedidos, detalles, logistica, tiendas, novedades);
        }
    }

}
