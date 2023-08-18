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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

builder.Host.ConfigureSerilog().UseMassTransit((hostContext, x) =>
{

    x.UsingInMemory();

    x.AddRider(r =>
    {
        r.AddKafkaComponents();
        
        r.AddMongoDbConfiguration(hostContext.Configuration);

        r.AddConsumer<PedidoConsumer>();

        var topicConsumer = hostContext.Configuration.GetValue<string>("Topics:ConsumerTopicName");

        var topicProducer = hostContext.Configuration.GetValue<string>("Topics:ProducerTopicName");
        
        r.AddProducer<string, StatusEstoque>(topicProducer);

        r.UsingKafka((context, cfg) =>
        {

            cfg.ClientId = "EstoqueApi";


            cfg.TopicEndpoint<string, PedidoDTO>(topicConsumer, "teste", e =>
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
