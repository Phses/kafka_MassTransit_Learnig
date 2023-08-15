using MassTransit;
using Pedido.Domain.DTO;

namespace Pedido.Domain.AsyncServices
{
    public class MessageBusClient : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MessageBusClient(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var producer = scope.ServiceProvider.GetService<ITopicProducer<string, PedidoDTO>>();
                await Produce(producer, stoppingToken);
            }catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível criar o producer --> {ex.Message}");
            }
        }

        private async Task Produce(ITopicProducer<string, PedidoDTO> producer, CancellationToken stoppingToken)
        {
            var message = new PedidoDTO()
            {
                Id = 1,
                IdProduto = 2,
                Qtde = 3,
                ValorTotal = 400,
            };

            await producer.Produce("", message, stoppingToken);

            Console.WriteLine("Mensagem enviada");
        }
    }
}
