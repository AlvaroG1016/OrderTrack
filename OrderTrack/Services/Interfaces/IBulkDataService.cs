using OrderTrack.Models.Domain;

namespace OrderTrack.Services.Interfaces
{
    public interface IBulkDataService
    {
        Task InsertarPorLotesAsync<T>(List<T> data, int batchSize) where T : class;
        Task<List<string?>> GetProductosBD();
        List<Producto> FiltrarProductosBD(List<Producto> productos, List<string> skusExistentes);
        List<Producto> FiltrarProductosMemoria(List<Producto> productos);
        Task<Dictionary<string, int>> MapeoIdProducto(List<Producto> productosNuevos);
    }
}
