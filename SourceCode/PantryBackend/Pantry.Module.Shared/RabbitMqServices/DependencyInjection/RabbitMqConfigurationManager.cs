using Microsoft.Extensions.Configuration;
using Pantry.Module.Shared.RabbitMqServices.Model;

namespace Pantry.Module.Shared.RabbitMqServices.DependencyInjection;
public static class RabbitMqConfigurationManager
{
    public static RabbitMqConfiguration CreateRabbitMqConfiguration(IConfiguration configuration)
    {
        var host = configuration["RabbitMQ:Host"];
        var port = configuration["RabbitMQ:Port"];
        var user = configuration["RabbitMQ:User"];
        var password = configuration["RabbitMQ:Password"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
        {
            throw new Exception("RabbitMQ configuration is not set");
        }

        var rabbitMqConfiguration = new RabbitMqConfiguration { Host = host, User = user, Password = password, Port = port };
        return rabbitMqConfiguration;
    }
}
