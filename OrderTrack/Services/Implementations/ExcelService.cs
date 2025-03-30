using ExcelDataReader;
using OrderTrack.Models.DTO;
using OrderTrack.Services.Interfaces;
using System.Data;

namespace OrderTrack.Services.Implementations
{
    public class ExcelService : IExcelService
    {
        public async Task<(List<PedidoDto>, List<DetallePedidoDto>, List<LogisticaDto>, List<TiendaDto>, List<NovedadDto>,
            List<ClienteDTO>, List<ProductoDTO>)> LeerExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("El archivo es inválido.");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var reader = ExcelReaderFactory.CreateReader(stream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true 
                }
            });
            var table = dataSet.Tables[0];

            var pedidos = new List<PedidoDto>();
            var detalles = new List<DetallePedidoDto>();
            var logistica = new List<LogisticaDto>();
            var tiendas = new List<TiendaDto>();
            var novedades = new List<NovedadDto>();
            var clientes = new List<ClienteDTO>();
            var productos = new List<ProductoDTO>();
            foreach (DataRow row in table.Rows)
            {
                // TODO: Validar que los int, datetime devuelvan nulos y verificar en bd si recibe o no null y como manejarlos.
                // DEBE retornar null para evitar valores inconsistentes

                int.TryParse(row[1]?.ToString(), out int pedidoId);
                TimeOnly.TryParse(row[2]?.ToString(), out TimeOnly horaPedido);
                DateOnly.TryParse(row[3]?.ToString(), out DateOnly fechaPedido);
                string nombreCliente = row[4]?.ToString() ?? "Cliente sin nombre";
                string telefonoCliente = row[5]?.ToString() ?? "Cliente sin telefono";
                string emailCliente = row[6]?.ToString() ?? "Cliente sin email";
                string numeroGuia = row[9]?.ToString() ?? "Orden sin numero de guía";
                string estadoPedido = row[10]?.ToString() ?? "Orden sin estado";
                string tipoEnvio = row[11]?.ToString() ?? "Orden sin tipo de envío";
                string departamentoDestino = row[12]?.ToString() ?? "Sin departamento destino";
                string ciudadDestino = row[13]?.ToString() ?? "Sin ciudad destino";
                string direccionDestino = row[14]?.ToString() ?? "Sin dirección destino";
                string notas = row[15]?.ToString() ?? "Sin notas";
                string transportadora = row[16]?.ToString() ?? "Sin transportadora";

                decimal.TryParse(row[17]?.ToString(), out decimal precioTotal);
                decimal.TryParse(row[18]?.ToString(), out decimal ganancia);
                decimal.TryParse(row[19]?.ToString(), out decimal precioFlete);
                decimal.TryParse(row[20]?.ToString(), out decimal costoDevolucionFlete);
                decimal.TryParse(row[21]?.ToString(), out decimal comision);
                int.TryParse(row[22]?.ToString(), out int porcentajeComision);
                decimal.TryParse(row[23]?.ToString(), out decimal precioProovedor);
                decimal.TryParse(row[24]?.ToString(), out decimal precioProovedorCantidad);
                int.TryParse(row[25]?.ToString(), out int productoId);
                string SKU = row[26]?.ToString() ?? "Sin SKU";
                int.TryParse(row[27]?.ToString(), out int variacionProductoId);
                string nombreProducto = row[28]?.ToString() ?? "Producto sin nombre";
                string variacionProducto = row[29]?.ToString() ?? "";
                int.TryParse(row[30]?.ToString(), out int cantidadProductos);
                string novedad = row[31]?.ToString() ?? "";
                string novedadSolucionada = row[32]?.ToString() ?? "";
                TimeOnly.TryParse(row[33]?.ToString(), out TimeOnly horaNovedad);
                DateOnly.TryParse(row[34]?.ToString(), out DateOnly fechaNovedad);
                string solucion = row[35]?.ToString() ?? "";
                TimeOnly.TryParse(row[36]?.ToString(), out TimeOnly horaSolucion);
                DateOnly.TryParse(row[37]?.ToString(), out DateOnly fechaSolucion);
                string observacion = row[38]?.ToString() ?? "";
                TimeOnly.TryParse(row[39]?.ToString(), out TimeOnly horaUltimoMovimiento);
                DateOnly.TryParse(row[40]?.ToString(), out DateOnly fechaUltimoMovimiento);
                string ultimoMovimiento = row[41]?.ToString() ?? "";
                string conceptoUltimoMovimiento = row[42]?.ToString() ?? "";
                string ubicacionUltimoMovimiento = row[43]?.ToString() ?? "";
                string vendedor = row[44]?.ToString() ?? "";
                string tipoTienda = row[45]?.ToString() ?? "";
                string tienda = row[46]?.ToString() ?? "";
                int.TryParse(row[47]?.ToString(), out int idOrdenTienda);
                int.TryParse(row[48]?.ToString(), out int numeroPedidoTienda);
                string tags = row[49]?.ToString() ?? "";
                DateOnly.TryParse(row[50]?.ToString(), out DateOnly fechaGuiaGenerada);




                // Agregar a las listas
                pedidos.Add(new PedidoDto { IdPedido = pedidoId, HoraPedido = horaPedido, FechaPedido = fechaPedido, Estado = estadoPedido,
                    Notas = notas, Comision = comision, PtjComision = porcentajeComision, Vendedor = vendedor, IdOrdenTienda = idOrdenTienda,
                    NumeroPedidoTienda = numeroPedidoTienda, Tags = tags,                    

                });
                detalles.Add(new DetallePedidoDto { IdPedidoInterno = pedidoId, PrecioTotal = precioTotal,
                Ganancia = ganancia, PrecioProovedor = precioProovedor, PrecioProovedorCantidad = precioProovedorCantidad,
                Cantidad = cantidadProductos, SKUTemp = SKU 

                });
                logistica.Add(new LogisticaDto 
                { IdPedidoInterno = pedidoId, NumeroGuia = numeroGuia, TipoEnvio = tipoEnvio,
                  Departamento = departamentoDestino, Ciudad = ciudadDestino, Direccion = direccionDestino
                  ,Transportadora = transportadora, PrecioFlete = precioFlete, CostoDevolucionFlete = costoDevolucionFlete,
                  HoraUltimoMov = horaUltimoMovimiento, FechaUltimoMov = fechaUltimoMovimiento, UltimoMovimiento = ultimoMovimiento,
                  ConceptoUltimoMovimiento = conceptoUltimoMovimiento, UbicacionUltimoMovimiento = ubicacionUltimoMovimiento,
                  FechaGuiaGenerada = fechaGuiaGenerada
                });
                tiendas.Add(new TiendaDto { TipoTienda = tipoTienda, Tienda1 = tienda});
                novedades.Add(new NovedadDto { Novedad = novedad, NovedadSolucionada = novedadSolucionada, HoraNovedad = horaNovedad, 
                    FechaNovedad = fechaNovedad, Solucion = solucion, HoraSolucion = horaSolucion, FechaSolucion = fechaSolucion 
                });
                clientes.Add(new ClienteDTO { Nombre = nombreCliente, Telefono = telefonoCliente, Email = emailCliente });

                productos.Add(new ProductoDTO { IdProductoExcel= productoId, Sku = SKU, Nombre = nombreProducto, VariacionId = variacionProductoId,
                    VariacionProducto = variacionProducto });
            }


            return (pedidos, detalles, logistica, tiendas, novedades, clientes, productos);
        }
    }

}
