using System;
using System.Collections.Generic;

namespace OrderTrack.Models.Domain;

public partial class Novedade
{
    public int IdNovedad { get; set; }

    public int IdPedido { get; set; }

    public string? Novedad { get; set; }

    public string? NovedadSolucionada { get; set; }

    public TimeOnly? HoraNovedad { get; set; }

    public DateOnly? FechaNovedad { get; set; }

    public string? Solucion { get; set; }

    public DateOnly? FechaSolucion { get; set; }

    public TimeOnly? HoraSolucion { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
