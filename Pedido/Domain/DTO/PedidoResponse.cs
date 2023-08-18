using Pedido.Domain.Enums;

namespace Pedido.Domain.DTO
{
    public class PedidoResponse
    {
        public string? Id { get; set; }
        public IEnumerable<string> ProdutosIds { get; set; }
        public StatusPedido StatusPedido { get; set; }
        public double ValorTotal { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
