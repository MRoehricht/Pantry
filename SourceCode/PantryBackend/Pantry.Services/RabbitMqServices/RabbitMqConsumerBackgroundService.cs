using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pantry.Services.RabbitMqServices.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pantry.Shared.Models.MessageModes;
using Pantry.Shared.Models.RecipeModels;
using Microsoft.Extensions.DependencyInjection;

namespace Pantry.Services.RabbitMqServices;
public class RabbitMqConsumerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConsumerBackgroundService(IServiceProvider serviceProvider,  ILoggerFactory loggerFactory, RabbitMqConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = loggerFactory.CreateLogger<RabbitMqConsumerBackgroundService>();

        //InitRabbitMQ
        var factory = new ConnectionFactory() { HostName = configuration.Host, Port = configuration.Port, UserName = configuration.User, Password = configuration.Password };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
        _channel.QueueDeclare(_configuration.QueueName, false, false, false, null);
        _channel.QueueBind(_configuration.QueueName, "demo.exchange", "demo.queue.*", null);
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

        _channel.BasicConsume(_configuration.QueueName, false, consumer);
        return Task.CompletedTask;
    }

    private void HandleMessage(string content) 
    {
        // we just print this message   
        _logger.LogInformation($"consumer received {content}");


        using IServiceScope scope = _serviceProvider.CreateScope();
        try {
            var consumerService = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumerService>();
       
            var doc = JsonDocument.Parse(content);
            var type = (MessageType)doc.RootElement.GetProperty("Type").GetInt32();


            if (type == MessageType.RegisterGood || type == MessageType.UpdateIngredientName || type == MessageType.MinimizeGoodsQuantity) {
                var message = JsonSerializer.Deserialize<Message<Ingredient>>(content);
                consumerService.ProcessMessage(message, type);
            } else if (type == MessageType.RecipeIsDeleted || type == MessageType.MealWasCooked) {
                var message = JsonSerializer.Deserialize<Message<Guid>>(content);
                consumerService.ProcessMessage(message, type);
            } 
        }
        catch (InvalidOperationException ex) {
            _logger.LogError($"{ex.Message}");
        }

        
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
