using System;
using System.Collections.Generic;

namespace OrderTrack.Models.Domain;

public partial class Tienda
{
    public int IdTienda { get; set; }

    public string TipoTienda { get; set; } = null!;

    public string Tienda1 { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
