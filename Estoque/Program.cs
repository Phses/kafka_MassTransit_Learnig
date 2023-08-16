using Estoque;
using Estoque.Configuration;
using Estoque.Domain.DTOs;
using Estoque.Domain.Entities;
using Estoque.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();


builder.Services.AddControllers();

builder.Host.ConfigureSerilog().UseMassTransit((hostContext, x) =>
{

    x.UsingInMemory();

    x.AddRider(r =>
    {
        r.AddKafkaComponents();
        
        r.AddMongoDbConfiguration(hostContext.Configuration);

        r.AddConsumer<PedidoConsumer>();

        var topic = hostContext.Configuration.GetValue<string>("Topics:ConsumerTopicName");

        r.UsingKafka((context, cfg) =>
        {

            cfg.ClientId = "EstoqueApi";

            cfg.TopicEndpoint<string, PedidoDTO>(topic, "teste", e =>
            {
                e.ConfigureConsumer<PedidoConsumer>(context);
            });

            cfg.Host(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
