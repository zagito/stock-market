using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MassTransit;

namespace MessageBroker.MassTransit;

public static class Extentions
{
    public static IServiceCollection AddMessageBroker
       (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            var t = configuration["MessageBroker:Host"];

            if (assembly != null)
                config.AddConsumers(assembly);

            config.UsingRabbitMq((context, configurator) =>
            {

                configurator.Host(configuration.GetConnectionString("stock-mq"));
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
