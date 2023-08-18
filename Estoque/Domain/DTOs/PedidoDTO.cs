using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using MongoDB.Bson;

namespace Estoque.Domain.DTOs
{
    public class PedidoDTO
    {
        public string Id { get; set; }
        public IEnumerable<string> ProdutosIds { get; set; }
    }
}
