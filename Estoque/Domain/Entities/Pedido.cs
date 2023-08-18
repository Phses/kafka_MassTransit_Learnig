using Estoque.Domain.Enums;
using MongoDB.Bson;

namespace Estoque.Domain.Entities
{
    public class Pedido
    {
        public IEnumerable<ObjectId> ProdutosIds { get; set; }
    }
}
