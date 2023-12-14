using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pantry.Services.RabbitMqServices.Model;

namespace Pantry.Services.RabbitMqServices.DependencyInjection;
public static class RabbitMqServicesCollectionExtension
{
    public static IServiceCollection AddRabbitMqServices(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration["RabbitMQ:Host"];
        var port = configuration["RabbitMQ:Port"];
        var user = configuration["RabbitMQ:User"];
        var password = configuration["RabbitMQ:Password"];
        var queueName = configuration["RabbitMQ:QueueName"];

        var rabbitMqConfiguration = new RabbitMqConfiguration { Host = host, User = user, Password = password, Port = int.Parse(port), QueueName = queueName };
        services.AddSingleton<RabbitMqConfiguration>(rabbitMqConfiguration);
        services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
        return services;
    }
}
