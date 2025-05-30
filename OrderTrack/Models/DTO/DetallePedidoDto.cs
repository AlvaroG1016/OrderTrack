﻿namespace OrderTrack.Models.DTO
{
    public class DetallePedidoDto
    {
        public int IdDetalle { get; set; }

        public int IdPedidoInterno { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }

        public decimal? Ganancia { get; set; }

        public decimal? PosibleGanancia { get; set; }

        public decimal? PrecioProovedor { get; set; }

        public decimal? PrecioProovedorCantidad { get; set; }

        public string? SKUTemp { get; set; }
        public decimal? PrecioFlete { get; set; }



    }
}
