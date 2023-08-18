using MongoDB.Bson;

namespace Pedido.Domain.Entities
{
    public class Estoque
    {
        public ObjectId Id { get; set; }
        public string ProdutoId { get; set; }
        public int Qtde { get; set; }
    }
}
