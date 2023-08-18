using AutoMapper;
using Estoque.Domain.DTOs;
using Estoque.Domain.Entities;
using MongoDB.Bson;

namespace Estoque.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<string, ObjectId>().ConvertUsing(s => ObjectId.Parse(s));

            CreateMap<PedidoDTO, Pedido>()
                .ForMember(dest => dest.ProdutosIds, opt => opt.MapFrom(src => src.ProdutosIds.Select(ObjectId.Parse)))
                .ReverseMap();
        }
    }
}
