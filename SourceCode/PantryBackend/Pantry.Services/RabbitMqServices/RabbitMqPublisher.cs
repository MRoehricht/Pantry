using Pantry.Services.RabbitMqServices.Model;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Pantry.Services.RabbitMqServices;
public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _conn;
    private readonly IModel _channel;

    public RabbitMqPublisher(RabbitMqConfiguration configuration)
    {
        _factory = new ConnectionFactory() { HostName = configuration.Host, Port = configuration.Port, UserName = configuration.User, Password = configuration.Password };
        _conn = _factory.CreateConnection();
        _channel = _conn.CreateModel();
        _channel.QueueDeclare(queue: "Pantry", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendMessage<T>(T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "", routingKey: "Pantry", basicProperties: null, body: body);
    }
}
