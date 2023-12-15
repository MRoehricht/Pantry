using Pantry.Services.RabbitMqServices.Model;
using Pantry.Shared.Models.MessageModes;
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
        _channel.QueueDeclare(queue: configuration.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendMessage<T>(T message, MessageType messageType = MessageType.None) {
        var messageObject = new Message<T> { Type = messageType, Content = message };
        var json = JsonSerializer.Serialize(messageObject);
        var body = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "", routingKey: messageType.GetDestinationQueue(), basicProperties: null, body: body);
    }

    private string GetQueueName(MessageType messageType) {
        switch (messageType) {
            case MessageType.None:
                return "Pantry";
            case MessageType.RegisterGood:
                var test2 = messageType.GetDestinationQueue();
                return "Pantry.Api";
            case MessageType.UpdateIngredientName:
                var test = messageType.GetDestinationQueue();
                return "Pantry.Recipe.Api";
            default:
                throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
        }
    }
}
