namespace OrderTrack.Services.Interfaces
{
    public interface ICargaDatosService
    {
        Task<string> ProcesarCarga(IFormFile file);
    }
}
