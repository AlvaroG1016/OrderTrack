namespace OrderTrack.Models.DTO
{
    public class LogisticaDto
    {
        public int IdLogistica { get; set; }

        public int IdPedidoInterno { get; set; }

        public string? NumeroGuia { get; set; }

        public DateOnly? FechaGuiaGenerada { get; set; }

        public string? UbicacionUltimoMovimiento { get; set; }

        public string TipoEnvio { get; set; } = null!;

        public string? Departamento { get; set; }

        public string? Ciudad { get; set; }

        public string? Direccion { get; set; }

        public string? Notas { get; set; }

        public string? Transportadora { get; set; }

        public string? ConceptoUltimoMovimiento { get; set; }

        public string? UltimoMovimiento { get; set; }
    }
}
