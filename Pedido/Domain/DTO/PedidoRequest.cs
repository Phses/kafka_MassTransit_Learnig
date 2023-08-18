using MongoDB.Bson;
using Pedido.Domain.Entities;
using Pedido.Domain.Enums;

namespace Pedido.Domain.DTO
{
    public class PedidoRequest
    {
        public IEnumerable<string> ProdutosIds { get; set; }
    }
}
