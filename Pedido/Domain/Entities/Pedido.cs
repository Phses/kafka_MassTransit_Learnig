namespace Pedido.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public int Qtde { get; set; }
        public double ValorTotal { get; set; }
    }
}
