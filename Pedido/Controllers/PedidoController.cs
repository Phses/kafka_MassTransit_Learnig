using Confluent.Kafka;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using Pedido.Domain.AsyncServices;
using Pedido.Domain.DTO;

namespace Pedido.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(ILogger<PedidoController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostPedidoAsync([FromBody] PedidoDTO pedido, [FromServices] ITopicProducer<string, PedidoDTO> producer)
        {
            try
            {
                await producer.Produce("", pedido);
                Console.WriteLine($"Produced message to topic");
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mensagem nao enviada {ex.Message}");
                return BadRequest();
            }
        }

    }
}
