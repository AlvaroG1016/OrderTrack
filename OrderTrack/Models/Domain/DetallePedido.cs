using System;
using System.Collections.Generic;

namespace OrderTrack.Models.Domain;

public partial class DetallePedido
{
    public int IdDetalle { get; set; }

    public int IdPedidoInterno { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal PrecioTotal { get; set; }

    public decimal? Ganancia { get; set; }

    public decimal? PosibleGanancia { get; set; }

    public decimal? PrecioProovedor { get; set; }

    public decimal? PrecioProovedorCantidad { get; set; }

    public virtual Pedido IdPedidoInternoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
