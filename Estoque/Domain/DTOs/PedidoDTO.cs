namespace Estoque.Domain.DTOs
{
    public class PedidoDTO
    {

        public int Id { get; set; }
        public int IdProduto { get; set; }
        public int Qtde { get; set; }
        public double ValorTotal { get; set; }

    }
}
