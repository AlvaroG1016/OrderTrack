namespace OrderTrack.Models.CustomResponses
{
    public class GeneralResponse
    {
        public bool Result { get; set; }
        public List<Object> Data { get; set; }
        public string Message { get; set; }
    }
}
