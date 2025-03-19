using OrderTrack.Models.Domain;
using OrderTrack.Services.Interfaces;
using EFCore.BulkExtensions;


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
    }
}
