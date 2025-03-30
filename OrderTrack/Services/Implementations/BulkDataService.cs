using OrderTrack.Models.Domain;
using OrderTrack.Services.Interfaces;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;


namespace OrderTrack.Services.Implementations
{
    public class BulkDataService : IBulkDataService
    {
        private readonly OrdertrackContext _context;
        private const int MAX_PARALLELISM = 4; // Control de carga en la BD

        public BulkDataService(OrdertrackContext context)
        {
            _context = context;
        }

        public async Task InsertarPorLotesAsync<T>(List<T> data, int batchSize) where T : class
        {
            var batches = data.Chunk(batchSize);

            await Parallel.ForEachAsync(batches, new ParallelOptions { MaxDegreeOfParallelism = MAX_PARALLELISM }, async (batch, _) =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await _context.BulkInsertAsync(batch);
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<List<string?>> GetProductosBD()
        {
            var skusExistentes = await _context.Productos
               .Select(p => p.Sku)
               .ToListAsync();

            return skusExistentes;
        }

        public List<Producto> FiltrarProductosBD(List<Producto> productos, List<string> skusExistentes)
        {
            var productosNuevos = productos
               .Where(p => !skusExistentes.Contains(p.Sku))
               .ToList();

            return productosNuevos;
        }

        public List<Producto> FiltrarProductosMemoria(List<Producto> productos)
        {
            var productosUnicos = productos
            .GroupBy(p => p.Sku)
            .Select(g => g.First())
            .ToList();

            return productosUnicos;
        }

        public async Task<Dictionary<string, int>> MapeoIdProducto(List<Producto> productosNuevos)
        {
            var productoIdMap = await _context.Productos
                .Where(p => p.Sku != null && productosNuevos.Select(x => x.Sku).Contains(p.Sku))
                .ToDictionaryAsync(p => p.Sku!, p => p.IdProducto); // El `!` indica que no será null

            return productoIdMap;
        }


    }
}
