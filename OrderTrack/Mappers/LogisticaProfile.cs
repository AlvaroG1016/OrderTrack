using AutoMapper;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class LogisticaProfile : Profile
    {
        public LogisticaProfile()
        {
            CreateMap<LogisticaDto, Logistica>();
        }
    }
}
