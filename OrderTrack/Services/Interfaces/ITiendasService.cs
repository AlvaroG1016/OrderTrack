using OrderTrack.Models.Domain;

namespace OrderTrack.Services.Interfaces
{
    public interface ITiendasService
    {
        List<Tienda> FiltrarTiendasMemoria(List<Tienda> tiendas);
        Task<Dictionary<string, int>> MapeoIdTienda();
    }
}
