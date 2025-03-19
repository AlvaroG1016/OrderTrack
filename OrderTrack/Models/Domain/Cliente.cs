using System;
using System.Collections.Generic;

namespace OrderTrack.Models.Domain;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public string? TipoIdentificacion { get; set; }

    public string? NumeroIdentificacion { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
