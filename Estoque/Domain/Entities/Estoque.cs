using MongoDB.Bson;

namespace Estoque.Domain.Entities
{
    public class EstoqueEntitie
    {
        public ObjectId Id { get; set; }
        public ObjectId ProdutoId { get; set; }
        public int Qtde { get; set; }
    }
}
