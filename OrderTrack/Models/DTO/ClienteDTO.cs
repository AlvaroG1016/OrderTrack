namespace OrderTrack.Models.DTO
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? Email { get; set; }

        public string? TipoIdentificacion { get; set; }

        public string? NumeroIdentificacion { get; set; }
    }
}
