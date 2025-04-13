namespace OrderTrack.Models.DTO.Response
{
    public class ProductoMasVendidoDto
    {
        public string NombreProducto { get; set; } = null!;
        public int TotalVendido { get; set; }
        public decimal Utilidad { get; set; }
    }
}
