using AutoMapper;
using MongoDB.Bson;
using Pedido.Domain.DTO;
using Pedido.Domain.Entities;

namespace Pedido.Profiles
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<string, ObjectId>().ConvertUsing(s => ObjectId.Parse(s));

            CreateMap<ObjectId, string>().ConvertUsing(s => s.ToString());

            CreateMap<PedidoRequest, PedidoEntitie>()
                .ForMember(dest => dest.ProdutosIds, opt => opt.MapFrom(src => src.ProdutosIds.Select(ObjectId.Parse)))
                .ReverseMap();
            CreateMap<PedidoEntitie, PedidoResponse>()
                .ForMember(dest => dest.ProdutosIds, opt => opt.MapFrom(src => src.ProdutosIds.Select(id => id.ToString())))
                .ReverseMap();
        }
    }
}
