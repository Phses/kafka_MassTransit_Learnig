using AutoMapper;
using Confluent.Kafka;
using Estoque.Domain.DTOs;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Driver;

namespace Estoque.Services
{
    public class PedidoConsumer : IConsumer<PedidoDTO>
    {
        private readonly IMongoCollection<EstoqueEntitie> _collection;
        private readonly ILogger<PedidoConsumer> _logger;
        private readonly ITopicProducer<string, StatusEstoque> _producer;
        private readonly IMapper _mapper;

        public PedidoConsumer(IMongoCollection<EstoqueEntitie> collection, ILogger<PedidoConsumer> logger, ITopicProducer<string, StatusEstoque> producer, IMapper mapper)
        {
            _collection = collection;
            _logger = logger;
            _producer = producer;
            _mapper = mapper;

        }
        public async Task Consume(ConsumeContext<PedidoDTO> context)
        {
            _logger.Log(LogLevel.Information, $"Mensagem recebida - Pedido {context.Message.Id}");
            using var sessionHandle = await _collection.Database.Client.StartSessionAsync(cancellationToken: context.CancellationToken);

            sessionHandle.StartTransaction();

            var statusEstoque = new StatusEstoque();

            statusEstoque.PedidoId = context.Message.Id;

            var pedido = _mapper.Map<Pedido>(context.Message);

            bool estoqueOk = false;

            foreach (var id in pedido.ProdutosIds)
            {
                var filter = Builders<EstoqueEntitie>.Filter.Eq(e => e.ProdutoId, id);
                var estoque = await _collection.Find(filter).FirstOrDefaultAsync();
                if (estoque.Qtde > 0)
                {
                    estoqueOk = true;
                    estoque.Qtde -= 1;
                    var update = Builders<EstoqueEntitie>.Update.Set(e => e.Qtde, estoque.Qtde);
                    await _collection.UpdateOneAsync(filter, update);
                } else { 
                    estoqueOk = false;
                }
            }
            if (estoqueOk)
            {

                statusEstoque.Status = "Ok";
            }
            else
            {
                statusEstoque.Status = "Nok";
            }


            await _producer.Produce("", statusEstoque);
            _logger.Log(LogLevel.Information, $"Mensagem enviada EstoqueAPI ");


            await sessionHandle.CommitTransactionAsync(context.CancellationToken);

        }
    }
}
