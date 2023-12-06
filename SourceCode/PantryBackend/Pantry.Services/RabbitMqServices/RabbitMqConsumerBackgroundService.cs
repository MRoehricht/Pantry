using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pantry.Services.RabbitMqServices.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Pantry.Services.RabbitMqServices;
public class RabbitMqConsumerBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConsumerBackgroundService(ILoggerFactory loggerFactory, RabbitMqConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<RabbitMqConsumerBackgroundService>();

        //InitRabbitMQ
        var factory = new ConnectionFactory() { HostName = configuration.Host, Port = configuration.Port, UserName = configuration.User, Password = configuration.Password };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
        _channel.QueueDeclare("Pantry", false, false, false, null);
        _channel.QueueBind("Pantry", "demo.exchange", "demo.queue.*", null);
        _channel.BasicQos(0, 1, false);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            // received message  
            var content = Encoding.UTF8.GetString(ea.Body.Span);

            // handle the received message  
            HandleMessage(content);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume("Pantry", false, consumer);
        return Task.CompletedTask;
    }

    private void HandleMessage(string content)
    {
        // we just print this message   
        _logger.LogInformation($"consumer received {content}");
    }

    private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
    private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}