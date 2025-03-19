using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderTrack.Models.Domain;
using EFCore.BulkExtensions;
using OrderTrack.Services.Interfaces;

namespace OrderTrack.Services.Implementations
{

    public class DataProcessingService : IDataProcessingService
    {
        private readonly OrdertrackContext _context;

        public DataProcessingService(OrdertrackContext context)
        {
            _context = context;
        }

        public async Task LimpiarBaseDeDatos()
        {
            // 🚀 Deshabilitar índices antes de borrar
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_Pedidos_Fecha ON Pedidos DISABLE;");
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_DetallePedidos_Pedido ON DetallePedidos DISABLE;");
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_DetallePedidos_Producto ON DetallePedidos DISABLE;");
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_Logistica_Pedido ON Logistica DISABLE;");

            await _context.Database.ExecuteSqlRawAsync("DELETE FROM DetallePedidos;");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Pedidos;");

            // 🛠 Opcional: Reiniciar los IDs autoincrementales
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('DetallePedidos', RESEED, 0);");
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Pedidos', RESEED, 0);");

        }

        public async Task ReconstruirIndices()
        {
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_Pedidos_Fecha ON Pedidos REBUILD;");
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_DetallePedidos_Pedido ON DetallePedidos REBUILD;");
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_DetallePedidos_Producto ON DetallePedidos REBUILD;");
            await _context.Database.ExecuteSqlRawAsync("ALTER INDEX idx_Logistica_Pedido ON Logistica REBUILD;");
        }
    }

}
