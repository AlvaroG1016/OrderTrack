using AutoMapper;
using OrderTrack.Models.Domain;
using OrderTrack.Models.DTO;

namespace OrderTrack.Mappers
{
    public class ProductoProfile:Profile
    {
        public ProductoProfile()
        {
            CreateMap<ProductoDTO, Producto>();
        }
    }
}
