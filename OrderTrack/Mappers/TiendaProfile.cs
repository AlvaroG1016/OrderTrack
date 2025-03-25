using AutoMapper;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class TiendaProfile:Profile
    {
        public TiendaProfile()
        {
            CreateMap<TiendaDto, Tienda>();
        }
    }
}
