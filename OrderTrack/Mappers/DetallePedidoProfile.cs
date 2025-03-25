using AutoMapper;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class DetallePedidoProfile : Profile
    {
        public DetallePedidoProfile()
        {
            CreateMap<DetallePedidoDto, DetallePedido>();
        }
    }
}
