using Estoque.Domain.DTOs;
using MassTransit;
using MongoDB.Driver;

namespace Estoque.Services
{
    public class PedidoConsumer : IConsumer<PedidoDTO>
    {
        private readonly IMongoCollection<PedidoDTO> _collection;

        public PedidoConsumer(IMongoCollection<PedidoDTO> collection)
        {
            _collection = collection;
        }
        public async Task Consume(ConsumeContext<PedidoDTO> context)
        {
            using var sessionHandle = await _collection.Database.Client.StartSessionAsync(cancellationToken: context.CancellationToken);

            sessionHandle.StartTransaction();

            Console.WriteLine(context.Message);

            await _collection.InsertOneAsync(context.Message);

            await sessionHandle.CommitTransactionAsync(context.CancellationToken);

        }
    }
}
