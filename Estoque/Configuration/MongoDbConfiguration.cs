using Estoque.Domain.DTOs;
using Estoque.Domain.Entities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Estoque.Configuration
{
    public static class MongoDbConfiguration
    {
        public static T AddMongoDbConfiguration<T>(this T configurator, IConfiguration configuration)
        where T : IServiceCollection
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(PedidoDTO)))
            {
                BsonClassMap.RegisterClassMap(new BsonClassMap<Pedido>(cfg =>
                {
                    cfg.AutoMap();
                    cfg.SetIgnoreExtraElements(true);
                }));
            }

            configurator.TryAddSingleton<IMongoClient>(_ => new MongoClient(configuration.GetConnectionString("MongoDb")));

            configurator.TryAddSingleton<IMongoCollection<PedidoDTO>>(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();

                return client.GetDatabase("KafkaDemo").GetCollection<PedidoDTO>("Pedido");
            });

            return configurator;
        }
    }
}
