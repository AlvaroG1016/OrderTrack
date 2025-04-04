using OrderTrack.Models.Domain;

namespace OrderTrack.Services.Interfaces
{
    public interface IClienteService
    {
        Task<Dictionary<string, int>> MapeoIdCliente();
        List<Cliente> FiltrarClientesMemoria(List<Cliente> clientes);
    }
}
