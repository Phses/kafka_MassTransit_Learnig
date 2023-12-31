﻿using MongoDB.Bson;
using Pedido.Domain.Enums;

namespace Pedido.Domain.Entities
{
    public class PedidoEntitie
    {
        public ObjectId Id { get; set; }
        public StatusPedido StatusPedido { get; set; }
        public IEnumerable<ObjectId> ProdutosIds { get; set; }
        public double ValorTotal { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
 