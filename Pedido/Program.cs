using MassTransit;
using Pedido.Config;
using Pedido.Domain.DTO;
using Pedido.Domain.Interfaces;
using Pedido.Domain.KafkaConfig;
using AutoMapper;
using Pedido.Domain.Entities;
using Pedido.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerGen();

var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>();
builder.Services.AddSingleton(kafkaOptions);


builder.Host
    .ConfigureSerilog()
    .UseMassTransit((hostContext, x) =>
    {
        x.AddMongoDbConfiguration(hostContext.Configuration);

        x.UsingInMemory();

        x.AddRider(r =>
        {
            r.AddKafkaComponents();

            var topicProducer = hostContext.Configuration.GetValue<string>("Topics:ProducerTopicName");
            if (topicProducer == default)
                throw new ConfigurationException("O nome do topico para o producer precisa ser configurado.");
            var topicConsumer = hostContext.Configuration.GetValue<string>("Topics:ConsumerTopicName");
            if (topicConsumer == default)
                throw new ConfigurationException("O nome do topico para o consumer precisa ser configurado.");

            r.AddConsumer<EstoqueConsumer>();

            r.AddProducer<string, PedidoResponse>(topicProducer);

            //r.AddProducer<string, ShipmentManifestReceived>("events.erp", (context, cfg) =>
            //{
            //    // Configure the AVRO serializer, with the schema registry client
            //    cfg.SetValueSerializer(new AvroSerializer<ShipmentManifestReceived>(context.GetRequiredService<ISchemaRegistryClient>()));
            //});

            r.UsingKafka((context, cfg) =>
            {
                cfg.ClientId = "PedidoApi";

                cfg.TopicEndpoint<string, StatusEstoque>(topicConsumer, "teste", e =>
                {
                    e.ConfigureConsumer<EstoqueConsumer>(context);
                });

                cfg.Host(context);
            });
        });
    });


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
