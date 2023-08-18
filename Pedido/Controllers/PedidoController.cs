using AutoMapper;
using Confluent.Kafka;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Pedido.Domain.DTO;
using Pedido.Domain.Entities;
using Pedido.Domain.Enums;

namespace Pedido.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<PedidoEntitie> _collectionPedido;
        private readonly IMongoCollection<Produto> _collectionProduto;

        public PedidoController(ILogger<PedidoController> logger, IMapper mapper, IMongoCollection<PedidoEntitie> collection, IMongoCollection<Produto> collectionProduto)
        {
            _logger = logger;
            _mapper = mapper;
            _collectionPedido = collection;
            _collectionProduto = collectionProduto;

        }

        [HttpPost]
        public async Task<IActionResult> PostPedidoAsync([FromBody] PedidoRequest pedido, [FromServices] ITopicProducer<string, PedidoResponse> producer)
        {
            try
            {
                var entitie = _mapper.Map<PedidoEntitie>(pedido);
                entitie.DataInclusao = DateTime.UtcNow;
                entitie.StatusPedido = StatusPedido.Criado;


                foreach (var id in entitie.ProdutosIds)
                {
                    var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
                    var produto = await _collectionProduto.Find(filter).FirstOrDefaultAsync();
                    entitie.ValorTotal += produto.Valor;
                }

                await _collectionPedido.InsertOneAsync(entitie);
                var pedidoResponse = _mapper.Map<PedidoResponse>(entitie);
                await producer.Produce("", pedidoResponse);
                _logger.Log(LogLevel.Information, $"Pedido:  criado");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warning, $"Pedido nao foi criado");
                return BadRequest();
            }
        }

    }
}
