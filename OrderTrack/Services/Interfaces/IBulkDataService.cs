namespace OrderTrack.Services.Interfaces
{
    public interface IBulkDataService
    {
        Task InsertarPorLotesAsync<T>(List<T> data, int batchSize) where T : class;
    }
}
