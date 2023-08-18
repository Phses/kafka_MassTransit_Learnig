using MongoDB.Bson;

namespace Estoque.Domain.DTOs
{
    public class EstoqueDTO
    {
        public ObjectId Id { get; set; }
        public string ProdutoId { get; set; }
        public int Qtde { get; set; }

    }
}