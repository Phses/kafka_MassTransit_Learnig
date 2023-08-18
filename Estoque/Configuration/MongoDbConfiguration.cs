using Estoque.Domain.DTOs;
using Estoque.Domain.Entities;
using Estoque.Domain.Enums;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Estoque.Configuration
{
    public static class MongoDbConfiguration
    {
        public static T AddMongoDbConfiguration<T>(this T configurator, IConfiguration configuration)
        where T : IServiceCollection
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(EstoqueEntitie)))
            {
                BsonClassMap.RegisterClassMap(new BsonClassMap<EstoqueEntitie>(cfg =>
                {
                    cfg.AutoMap();
                    cfg.SetIgnoreExtraElements(true);
                }));
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Pedido)))
            {
                BsonClassMap.RegisterClassMap(new BsonClassMap<Pedido>(cfg =>
                {
                    cfg.AutoMap();
                    cfg.SetIgnoreExtraElements(true);
                }));
            }

            configurator.TryAddSingleton<IMongoClient>(_ => new MongoClient(configuration.GetConnectionString("MongoDb")));

            configurator.TryAddSingleton(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();

                return client.GetDatabase("kafka_demo").GetCollection<EstoqueEntitie>("estoque");
            });

            return configurator;
        }
    }
}
