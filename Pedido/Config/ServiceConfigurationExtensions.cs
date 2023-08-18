using Serilog.Events;
using Serilog;
using MassTransit;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using Pedido.Domain.KafkaConfig;
using Pedido.Config;

namespace Pedido.Config
{
    public static class ServiceConfigurationExtensions
    {
        public static T AddKafkaComponents<T>(this T services)
        where T : IServiceCollection
        {
            services.AddOptions<KafkaOptions>().BindConfiguration("Kafka");

            return services;
        }
        public static void Host(this IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context)
        {
            var options = context.GetRequiredService<IOptions<KafkaOptions>>().Value;

            configurator.Host(options.BootStrapServer, h =>
            {
                h.UseSasl(s =>
                {
                    s.SecurityProtocol = SecurityProtocol.SaslSsl;
                    s.Mechanism = SaslMechanism.Plain;
                    s.Username = options.SaslUsername;
                    s.Password = options.SaslPassword;
                });
            });
        }

        public static T ConfigureSerilog<T>(this T builder)
        where T : IHostBuilder
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.UseSerilog();

            return builder;
        }
    }
}
