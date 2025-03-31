using System;
using System.Collections.Generic;

namespace OrderTrack.Models.Domain;

public partial class Pedido
{
    public int IdPedidoInterno { get; set; }

    public int IdPedido { get; set; }

    public DateOnly FechaPedido { get; set; }

    public TimeOnly? HoraPedido { get; set; }

    public int IdTienda { get; set; }

    public string IdOrdenTienda { get; set; } = null!;

    public long? NumeroPedidoTienda { get; set; }

    public string Estado { get; set; } = null!;

    public string Vendedor { get; set; } = null!;

    public string? Tags { get; set; }

    public int IdCliente { get; set; }

    public string? Notas { get; set; }

    public decimal? Comision { get; set; }

    public int? PtjComision { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Tienda IdTiendaNavigation { get; set; } = null!;

    public virtual ICollection<Logistica> Logisticas { get; set; } = new List<Logistica>();

    public virtual ICollection<Novedade> Novedades { get; set; } = new List<Novedade>();
}
