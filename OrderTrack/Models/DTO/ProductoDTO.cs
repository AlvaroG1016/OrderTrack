﻿namespace OrderTrack.Models.DTO
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Sku { get; set; }

        public int? IdProductoExcel { get; set; }

        public int? VariacionId { get; set; }

        public string? VariacionProducto { get; set; }
    }
}
