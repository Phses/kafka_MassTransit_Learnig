using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Pedido.Domain.Entities;
using Pedido.Domain.Enums;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Pedido.Config
{
    public static class MongoDbConfigurationExtensions
    {
        public static T AddMongoDbConfiguration<T>(this T configurator, IConfiguration configuration) where T : IServiceCollection
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(PedidoEntitie)))
            {
                BsonClassMap.RegisterClassMap(new BsonClassMap<PedidoEntitie>(cfg =>
                {
                    cfg.AutoMap();
                    cfg.SetIgnoreExtraElements(true);
                    cfg.MapMember(p => p.StatusPedido).SetSerializer(new EnumSerializer<StatusPedido>(BsonType.String));
                }));
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Produto)))
            {
                BsonClassMap.RegisterClassMap(new BsonClassMap<Produto>(cfg =>
                {
                    cfg.AutoMap();
                    cfg.SetIgnoreExtraElements(true);
                }));
            }

            var conecction = configuration.GetConnectionString("MongoDb");

            configurator.TryAddSingleton<IMongoClient>(_ => new MongoClient(configuration.GetConnectionString("MongoDb")));

            configurator.TryAddSingleton(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();

                return client.GetDatabase("kafka_demo").GetCollection<PedidoEntitie>("pedido");
            });

            configurator.TryAddSingleton(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();

                return client.GetDatabase("kafka_demo").GetCollection<Produto>("produto");
            });

            return configurator;

        }
    }
}