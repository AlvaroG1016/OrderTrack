using AutoMapper;
using OrderTrack.Models;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class PedidosProfile:Profile
    {
        public PedidosProfile()
        {
            CreateMap<PedidoDto, Pedido>();
        }
    }
}
