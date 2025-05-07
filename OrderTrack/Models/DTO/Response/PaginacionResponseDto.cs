namespace OrderTrack.Models.DTO.Response
{
    public class PaginacionResponseDto<T>
    {
        public int TotalRows { get; set; }
        public List<T> Data { get; set; } = new();
    }
}
