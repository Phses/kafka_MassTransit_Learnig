using MassTransit;
using Pedido;
using Pedido.Domain.AsyncServices;
using Pedido.Domain.DTO;
using Pedido.Domain.Interfaces;
using Pedido.Domain.KafkaConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>();
builder.Services.AddSingleton(kafkaOptions);



builder.Host
    .ConfigureSerilog()
    .UseMassTransit((hostContext, x) =>
    {
        x.UsingInMemory();

        x.AddRider(r =>
        {
            r.AddKafkaComponents();

            var topic = hostContext.Configuration.GetValue<string>("Topics:ProducerTopicName");
            if (topic == default)
                throw new ConfigurationException("O nome do topico para o producer precisa ser configurado.");

            r.AddProducer<string, PedidoDTO>(topic);

            //r.AddProducer<string, ShipmentManifestReceived>("events.erp", (context, cfg) =>
            //{
            //    // Configure the AVRO serializer, with the schema registry client
            //    cfg.SetValueSerializer(new AvroSerializer<ShipmentManifestReceived>(context.GetRequiredService<ISchemaRegistryClient>()));
            //});

            r.UsingKafka((context, cfg) =>
            {
                cfg.ClientId = "PedidoApi";

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
