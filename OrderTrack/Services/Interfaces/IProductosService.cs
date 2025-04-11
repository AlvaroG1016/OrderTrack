using OrderTrack.Models.Domain;

namespace OrderTrack.Services.Interfaces
{
    public interface IProductosService
    {
        Task<List<string>> GetClavesProductosBD();
        List<Producto> FiltrarProductosBD(List<Producto> productos, List<string> clavesExistentes);
        List<Producto> FiltrarProductosMemoria(List<Producto> productos);
        Task<Dictionary<string, int>> MapeoIdProducto(List<Producto> productosNuevos);
        Task<List<Producto>> GetAllProductos();
    }
}
