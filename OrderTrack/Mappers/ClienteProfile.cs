using AutoMapper;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class ClienteProfile:Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteDTO, Cliente>();
        }
    }
}
