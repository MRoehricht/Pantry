using Pantry.Shared.Models.MessageModes;

namespace Pantry.Services.RabbitMqServices {
    public interface IRabbitMqConsumerService{
    public Task ProcessMessage<T>(T message, MessageType type);
    }
}
