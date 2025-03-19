namespace OrderTrack.Models.DTO
{
    public class NovedadDto
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
    }
}
