namespace OrderTrack.Services.Interfaces
{
    public interface IDataProcessingService
    {
        Task LimpiarBaseDeDatos();
        Task ReconstruirIndices();
    }
}
