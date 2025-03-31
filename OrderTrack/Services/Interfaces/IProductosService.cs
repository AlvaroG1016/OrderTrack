using OrderTrack.Models.Domain;

namespace OrderTrack.Services.Interfaces
{
    public interface IProductosService
    {
        Task<List<string?>> GetSKUProductosBD();
        List<Producto> FiltrarProductosBD(List<Producto> productos, List<string> skusExistentes);
        List<Producto> FiltrarProductosMemoria(List<Producto> productos);
        Task<Dictionary<string, int>> MapeoIdProducto(List<Producto> productosNuevos);
        Task<List<Producto>> GetAllProductos();
    }
}
