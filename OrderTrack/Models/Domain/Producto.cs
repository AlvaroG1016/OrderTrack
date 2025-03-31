using System;
using System.Collections.Generic;

namespace OrderTrack.Models.Domain;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Sku { get; set; }

    public int? IdProductoExcel { get; set; }

    public int? VariacionId { get; set; }

    public string? VariacionProducto { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}
