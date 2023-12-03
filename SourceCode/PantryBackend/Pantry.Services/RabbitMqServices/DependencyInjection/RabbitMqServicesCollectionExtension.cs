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

        var rabbitMqConfiguration = new RabbitMqConfiguration { Host = host, User = user, Password = password, Port = int.Parse(port) };

        services.AddScoped<IRabbitMqPublisher>(_ => new RabbitMqPublisher(rabbitMqConfiguration));
        return services;
    }
}
