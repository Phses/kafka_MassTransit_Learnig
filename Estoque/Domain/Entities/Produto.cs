using MongoDB.Bson;

namespace Estoque.Domain.Entities
{
    public class Produto
    {
        public ObjectId Id { get; set; }
        public Guid SerialNumber { get; set; }
        public string Name { get; set; }
        public double Valor { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
