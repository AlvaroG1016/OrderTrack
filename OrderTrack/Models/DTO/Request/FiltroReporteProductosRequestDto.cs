namespace OrderTrack.Models.DTO.Request
{
    public class ProductoVentaDto
    {
        public string NombreProducto { get; set; } = null!;
        public int TotalVendido { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public class AgrupacionProductosDto
    {
        public string Agrupacion { get; set; } = null!; // "2025-W15" o "2025-03"
        public List<ProductoVentaDto> Productos { get; set; } = new();
    }
    public class FiltroReporteProductosDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string TipoAgrupacion { get; set; } = "semana"; // o "mes"
    }

}
