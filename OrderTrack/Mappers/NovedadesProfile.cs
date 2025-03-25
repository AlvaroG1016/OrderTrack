using AutoMapper;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class NovedadesProfile:Profile
    {
        public NovedadesProfile()
        {
            CreateMap<NovedadDto, Novedade>();
        }
    }
}
