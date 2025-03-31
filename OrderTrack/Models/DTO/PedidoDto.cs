namespace OrderTrack.Models.DTO
{
    public class PedidoDto
    {
        public int IdPedidoInterno { get; set; }

        public int IdPedido { get; set; }

        public DateOnly FechaPedido { get; set; }

        public TimeOnly? HoraPedido { get; set; }

        public int IdTienda { get; set; }

        public string? IdOrdenTienda { get; set; }

        public long? NumeroPedidoTienda { get; set; }

        public string Estado { get; set; } = null!;

        public string Vendedor { get; set; } = null!;

        public string? Tags { get; set; }

        public int IdCliente { get; set; }

        public string? Notas { get; set; }

        public decimal? Comision { get; set; }

        public int? PtjComision { get; set; }
        public string? NombreTiendaTemp { get; set; }

    }
}
