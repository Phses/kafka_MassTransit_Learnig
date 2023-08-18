using MassTransit;
using MongoDB.Bson;
using MongoDB.Driver;
using Pedido.Domain.Entities;
using Pedido.Domain.Enums;

namespace Pedido.Services
{
    public class EstoqueConsumer : IConsumer<StatusEstoque>
    {
        private readonly IMongoCollection<PedidoEntitie> _collection;
        private readonly ILogger<EstoqueConsumer> _logger;

        public EstoqueConsumer(IMongoCollection<PedidoEntitie> collection, ILogger<EstoqueConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<StatusEstoque> context)
        {
            _logger.Log(LogLevel.Information, $"Mensagem recebida - Pedido {context.Message.PedidoId}");

            using var sessionHandle = await _collection.Database.Client.StartSessionAsync(cancellationToken: context.CancellationToken);

            sessionHandle.StartTransaction();

            var statusEstoque = context.Message;

            var pedidoId = ObjectId.Parse(statusEstoque.PedidoId);

            var filter = Builders<PedidoEntitie>.Filter.Eq(p => p.Id, pedidoId);

            var statusPedido = StatusPedido.Confirmado;
            if(statusEstoque.Status == "Nok")
            {
                statusPedido = StatusPedido.Cancelado;
            }

            var update = Builders<PedidoEntitie>.Update.Set(p => p.StatusPedido, statusPedido).Set(p => p.DataAlteracao, DateTime.UtcNow);

            await _collection.UpdateOneAsync(filter, update);

            _logger.Log(LogLevel.Information, $"Status Pedido {statusEstoque.PedidoId} alterado");

            await sessionHandle.CommitTransactionAsync(context.CancellationToken);
        }
    }
}